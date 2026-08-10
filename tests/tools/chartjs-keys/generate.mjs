/*
 * Regenerates tests/Erkan.Blazor.Chartjs.Tests/ChartJs/chartjs-option-paths.json,
 * the allowlist ChartJsKeyValidationTests validates every [JsonPropertyName] against.
 *
 * Run it after bumping any library under src/wwwroot/lib:
 *
 *     cd tests/tools/chartjs-keys && npm install && npm run generate
 *
 * WHY THIS EXISTS
 * ---------------
 * A C# property can serialize to a key Chart.js never reads, or to the right key at the
 * wrong nesting level, and nothing at build or run time complains: Chart.js ignores keys
 * it does not know. Every such defect is a diff against the set of paths Chart.js
 * actually accepts, so this script materializes that set and the test diffs against it.
 *
 * SOURCE OF TRUTH
 * ---------------
 * Two independent sources, unioned, with the second cross-checking the first.
 *
 * 1. PRIMARY - the TypeScript declarations Chart.js and its plugins publish.
 *    They describe the exact option tree including nesting, which is what matters:
 *    `mode` is valid under plugins.zoom.zoom and invalid under plugins.zoom, and only a
 *    typed tree distinguishes those. The declarations are walked with the TypeScript
 *    compiler API, so generics, unions and intersections are resolved by tsc itself
 *    rather than by a regex.
 *
 *    src/wwwroot/lib/Chart.js/ does ship index.d.ts and types.d.ts, but both are
 *    three-line re-export stubs pointing at ./controllers/index.js, ./core/index.js and
 *    ./types/index.js - files the vendoring step did not copy. They resolve to nothing
 *    and carry no option names, so the declarations are taken from the npm packages
 *    instead, pinned in package.json to the exact versions vendored under
 *    src/wwwroot/lib. checkVersions() below fails the run if those two ever drift, so
 *    the types can never describe a different build than the one the wrapper ships.
 *
 * 2. SECONDARY - the live `defaults` objects of the vendored bundles themselves.
 *    The vendored UMD builds are loaded (Chart.js directly, the plugins in a vm sandbox
 *    with a browser-shaped global) and their defaults trees are read. This is the code
 *    that actually runs in the browser. It is used two ways: as a cross-check that the
 *    typed tree covers everything the shipped bundle defines, and as a union member so a
 *    key the declarations happen to omit cannot fail the test. Anything it contributes
 *    that the types did not is written to `runtimeOnlyPaths` so it is visible in review.
 *
 * WHAT THE OUTPUT DOES AND DOES NOT PROVE
 * ---------------------------------------
 * `paths` is every option path Chart.js 4.5.1 and the bundled plugins accept, relative
 * to the root of a chart config ("type", "data.labels", "options.plugins.zoom.zoom.mode").
 * A '*' segment is an index signature - any single segment matches it.
 *
 * It proves a key exists at that nesting level and nothing more. It does not check value
 * types, does not check that a key is meaningful for the particular chart type it is set
 * on (the eight chart types' option trees are unioned), and cannot see options read
 * dynamically by a plugin without a declaration for them.
 */

import ts from 'typescript';
import fs from 'node:fs';
import path from 'node:path';
import vm from 'node:vm';
import { createRequire } from 'node:module';
import { fileURLToPath } from 'node:url';

const here = path.dirname(fileURLToPath(import.meta.url));
const repo = path.resolve(here, '../../..');
const vendorDir = path.join(repo, 'src/wwwroot/lib');
const interopJs = path.join(repo, 'src/wwwroot/Chart.js');
const outFile = path.join(repo, 'tests/Erkan.Blazor.Chartjs.Tests/ChartJs/chartjs-option-paths.json');

const require_ = createRequire(path.join(here, 'noop.cjs'));

const CHART_TYPES = ['bar', 'bubble', 'doughnut', 'line', 'pie', 'polarArea', 'radar', 'scatter'];

/** Vendored bundle -> the banner comment its version is read from. */
const VENDORED = {
  'chart.js': 'Chart.js/chart.umd.js',
  'chartjs-plugin-zoom': 'chartjs-plugin-zoom/chartjs-plugin-zoom.js',
  'chartjs-plugin-datalabels': 'chartjs-plugin-datalabels/chartjs-plugin-datalabels.js',
  'chartjs-plugin-annotation': 'chartjs-plugin-annotation/chartjs-plugin-annotation.min.js',
  'chartjs-plugin-autocolors': 'chartjs-plugin-autocolors/chartjs-plugin-autocolors.min.js',
};

// ---------------------------------------------------------------------------
// 0. the npm declarations must describe the vendored bundles, not some other build
// ---------------------------------------------------------------------------

function vendoredVersion(relative) {
  const file = path.join(vendorDir, relative);
  const head = fs.readFileSync(file, 'utf8').slice(0, 2000);
  const m = head.match(/v(\d+\.\d+\.\d+)/);
  if (!m) throw new Error(`no version banner in ${relative}`);
  return m[1];
}

function checkVersions() {
  const versions = {};
  const problems = [];
  for (const [pkg, relative] of Object.entries(VENDORED)) {
    const vendored = vendoredVersion(relative);
    // read from disk rather than require(): chart.js's "exports" map does not expose
    // ./package.json, so a bare require of it throws ERR_PACKAGE_PATH_NOT_EXPORTED
    let installed = null;
    const manifest = path.join(here, 'node_modules', pkg, 'package.json');
    if (fs.existsSync(manifest)) installed = JSON.parse(fs.readFileSync(manifest, 'utf8')).version;
    versions[pkg] = { vendored, installed };
    if (installed !== vendored) {
      problems.push(`${pkg}: vendored ${vendored}, npm ${installed ?? '(not installed)'}`);
    }
  }
  if (problems.length) {
    throw new Error(
      'The pinned npm packages do not match the bundles vendored under src/wwwroot/lib, so the\n' +
      'type declarations would describe different code than the wrapper ships. Update the\n' +
      '"dependencies" block in package.json to the vendored versions and re-run npm install.\n  ' +
      problems.join('\n  '));
  }
  return versions;
}

// ---------------------------------------------------------------------------
// 1. walk the published TypeScript declarations
// ---------------------------------------------------------------------------

/**
 * Only types declared inside these packages are expanded. Without this the walk escapes
 * the option tree through members like ScriptableContext.chart and enumerates the whole
 * DOM, which both explodes and produces meaningless "option" names.
 */
const ALLOWED_DECL_FILE = /node_modules[\\/](chart\.js|chartjs-plugin-[a-z]+)[\\/]/;

const MAX_DEPTH = 9;

function walkDeclarations() {
  const entry = path.join(here, '.walk-entry.ts');
  fs.writeFileSync(entry, `
import { ChartConfiguration, LegendItem } from 'chart.js';
import 'chartjs-plugin-zoom';
import 'chartjs-plugin-datalabels';
import 'chartjs-plugin-annotation';
import 'chartjs-plugin-autocolors';
${CHART_TYPES.map(t => `declare const cfg_${t}: ChartConfiguration<'${t}'>;`).join('\n')}
declare const legendItem: LegendItem;
`);

  const program = ts.createProgram([entry], {
    target: ts.ScriptTarget.ES2020,
    module: ts.ModuleKind.ESNext,
    moduleResolution: ts.ModuleResolutionKind.Node10,
    strict: true,
    skipLibCheck: true,
    types: [],
  });
  const checker = program.getTypeChecker();
  const sf = program.getSourceFile(entry);

  const fatal = program.getSemanticDiagnostics(sf)
    .map(d => ts.flattenDiagnosticMessageText(d.messageText, ' '));
  if (fatal.length) {
    throw new Error('the declaration entry point did not type-check:\n  ' + fatal.join('\n  '));
  }

  const EMPTY = new Set();
  const PRIMITIVE =
    ts.TypeFlags.String | ts.TypeFlags.Number | ts.TypeFlags.Boolean |
    ts.TypeFlags.StringLiteral | ts.TypeFlags.NumberLiteral | ts.TypeFlags.BooleanLiteral |
    ts.TypeFlags.Null | ts.TypeFlags.Undefined | ts.TypeFlags.Void | ts.TypeFlags.Never |
    ts.TypeFlags.ESSymbol | ts.TypeFlags.Unknown | ts.TypeFlags.Any | ts.TypeFlags.BigInt |
    ts.TypeFlags.EnumLike;

  const memo = new Map();
  const active = new Set();

  const isClassLike = type =>
    (type.getSymbol()?.getDeclarations() ?? [])
      .some(d => ts.isClassDeclaration(d) || ts.isClassExpression(d));

  const fromAllowedFile = type => {
    const sym = type.getSymbol() ?? type.aliasSymbol;
    const files = (sym?.getDeclarations() ?? []).map(d => d.getSourceFile().fileName);
    // an anonymous type literal has no symbol of its own; it inherits its parent's context
    return files.length === 0 || files.some(f => ALLOWED_DECL_FILE.test(f));
  };

  /** Relative dotted paths beneath `type`, memoized per (type, remaining depth). */
  function expand(type, depth) {
    if (!type || depth <= 0) return EMPTY;
    if (type.getFlags() & PRIMITIVE) return EMPTY;

    if (type.isUnionOrIntersection()) {
      const out = new Set();
      for (const member of type.types) for (const p of expand(member, depth)) out.add(p);
      return out;
    }
    // a bare function is a leaf; a callable object still has properties worth walking
    if (type.getCallSignatures().length > 0 && type.getApparentProperties().length === 0) return EMPTY;

    // arrays are unwrapped before the declaration-site filter below, because Array itself
    // is declared in lib.es5.d.ts: filtering first would discard data.datasets[] whole
    if (checker.isArrayType(type)) {
      // an array of option objects occupies the same path as the array itself:
      // data.datasets[0].borderColor is data.datasets.borderColor here
      return expand(checker.getTypeArguments(type)[0], depth);
    }
    if (checker.isTupleType(type)) return EMPTY;
    if (isClassLike(type) || !fromAllowedFile(type)) return EMPTY;

    const key = `${type.id}:${depth}`;
    if (memo.has(key)) return memo.get(key);
    if (active.has(key)) return EMPTY; // recursive type; cut here, the outer frame has it
    active.add(key);

    const out = new Set();
    {
      const indexType = type.getStringIndexType();
      if (indexType) {
        out.add('*');
        for (const p of expand(indexType, depth - 1)) out.add(`*.${p}`);
      }
      for (const prop of type.getApparentProperties()) {
        const name = prop.getName();
        if (name.startsWith('_')) continue;
        const decl = prop.valueDeclaration ?? prop.declarations?.[0];
        if (!decl || !ALLOWED_DECL_FILE.test(decl.getSourceFile().fileName)) continue;
        out.add(name);
        const propType = checker.getTypeOfSymbolAtLocation(prop, decl);
        for (const p of expand(propType, depth - 1)) out.add(`${name}.${p}`);
      }
    }

    active.delete(key);
    memo.set(key, out);
    return out;
  }

  const typeOfDeclaration = name => {
    let found;
    ts.forEachChild(sf, node => {
      if (ts.isVariableStatement(node)) {
        for (const d of node.declarationList.declarations) {
          if (d.name.getText() === name) found = d;
        }
      }
    });
    if (!found) throw new Error(`declaration ${name} not found`);
    return checker.getTypeAtLocation(found.name);
  };

  const configPaths = new Set();
  for (const t of CHART_TYPES) {
    for (const p of expand(typeOfDeclaration(`cfg_${t}`), MAX_DEPTH)) configPaths.add(p);
  }
  const legendItemPaths = new Set(expand(typeOfDeclaration('legendItem'), 3));

  fs.unlinkSync(entry);
  return { configPaths, legendItemPaths };
}

// ---------------------------------------------------------------------------
// 2. read the live defaults out of the bundles the wrapper actually ships
// ---------------------------------------------------------------------------

function flattenDefaults(value, prefix, out, depth = 0) {
  if (depth > 8 || value === null || typeof value !== 'object' || Array.isArray(value)) return;
  for (const key of Object.keys(value)) {
    if (key.startsWith('_') || key.startsWith('$')) continue;
    const p = prefix ? `${prefix}.${key}` : key;
    out.add(p);
    flattenDefaults(value[key], p, out, depth + 1);
  }
}

function runtimeDefaults() {
  const paths = new Set();
  const Chart = require_(path.join(vendorDir, 'Chart.js/chart.umd.js'));

  const d = Chart.defaults;
  for (const key of Object.keys(d)) {
    if (key.startsWith('_') || ['controllers', 'scale', 'scales', 'datasets', 'overrides'].includes(key)) continue;
    paths.add(`options.${key}`);
    flattenDefaults(d[key], `options.${key}`, paths);
  }
  // every scale's defaults live at options.scales.<any id>
  flattenDefaults(d.scale, 'options.scales.*', paths);
  for (const scale of Object.values(d.scales ?? {})) flattenDefaults(scale, 'options.scales.*', paths);
  // per-controller dataset defaults are settable as options.datasets.<type> and on the dataset itself
  for (const [type, defs] of Object.entries(d.datasets ?? {})) {
    flattenDefaults(defs, `options.datasets.${type}`, paths);
    flattenDefaults(defs, 'data.datasets', paths);
  }
  for (const overrides of Object.values(Chart.overrides ?? {})) flattenDefaults(overrides, 'options', paths);

  // the plugin UMD builds fall through to their browser-global branch in a sandbox with
  // no module/exports/define, which is the only way to load them without a bundler
  const sandbox = {
    Chart, console, Math, Date, JSON, Object, Array, String, Number, Boolean, RegExp,
    Error, TypeError, Symbol, Map, Set, WeakMap, WeakSet, Promise, isNaN, parseFloat,
    parseInt, setTimeout, clearTimeout, requestAnimationFrame: () => 0,
    document: { createElement: () => ({ getContext: () => null, style: {} }), addEventListener() {}, documentElement: { style: {} } },
    navigator: { userAgent: 'node' },
  };
  sandbox.window = sandbox;
  sandbox.globalThis = sandbox;
  sandbox.self = sandbox;
  vm.createContext(sandbox);

  const loadInSandbox = relative => {
    const file = path.join(vendorDir, relative);
    vm.runInContext(fs.readFileSync(file, 'utf8'), sandbox, { filename: file });
  };
  loadInSandbox('hammer.js/hammer.js');
  loadInSandbox('chartjs-plugin-datalabels/chartjs-plugin-datalabels.js');
  loadInSandbox('chartjs-plugin-zoom/chartjs-plugin-zoom.js');
  loadInSandbox('chartjs-plugin-annotation/chartjs-plugin-annotation.min.js');
  loadInSandbox('chartjs-plugin-autocolors/chartjs-plugin-autocolors.min.js');

  const registered = [];
  for (const [id, plugin] of Object.entries(Chart.registry.plugins.items ?? {})) {
    registered.push(id);
    if (plugin?.defaults) flattenDefaults(plugin.defaults, `options.plugins.${id}`, paths);
  }
  for (const [global, id] of [['ChartDataLabels', 'datalabels'], ['ChartZoom', 'zoom'], ['ChartAnnotation', 'annotation'], ['autocolors', 'autocolors']]) {
    const plugin = sandbox[global];
    if (plugin?.defaults) {
      if (!registered.includes(id)) registered.push(id);
      flattenDefaults(plugin.defaults, `options.plugins.${id}`, paths);
    }
  }
  return { paths, registered: registered.sort() };
}

// ---------------------------------------------------------------------------
// 3. keys the wrapper's own interop layer strips before Chart.js sees the config
// ---------------------------------------------------------------------------

/**
 * The wrapper serializes markers such as hasCallback so its JS knows a .NET callback is
 * registered, then deletes them in chartSetup. Those keys are legitimately absent from
 * Chart.js's option tree, so the test must exempt them - but only the ones the interop
 * layer really deletes. Reading them out of src/wwwroot/Chart.js rather than hardcoding
 * a list means a marker added to the models without a matching delete fails the test.
 */
function strippedByInterop() {
  const source = fs.readFileSync(interopJs, 'utf8');
  const keys = new Set();
  for (const m of source.matchAll(/\bdelete\s+[A-Za-z_$][\w$?.]*\.([A-Za-z_$][\w$]*)\s*;/g)) {
    keys.add(m[1]);
  }
  return [...keys].sort();
}

// ---------------------------------------------------------------------------

const versions = checkVersions();
const { configPaths, legendItemPaths } = walkDeclarations();
const { paths: runtimePaths, registered } = runtimeDefaults();

const runtimeOnly = [...runtimePaths].filter(p => !configPaths.has(p)).sort();
const paths = new Set([...configPaths, ...runtimePaths]);

const output = {
  $comment: [
    'GENERATED FILE - do not edit by hand.',
    'Regenerate with: cd tests/tools/chartjs-keys && npm install && npm run generate',
    'Every option path Chart.js 4.5.1 and the bundled plugins accept, relative to the root of',
    "a chart configuration. A '*' segment matches any single segment (a JS index signature).",
  ],
  generator: 'tests/tools/chartjs-keys/generate.mjs',
  versions: Object.fromEntries(Object.entries(versions).map(([k, v]) => [k, v.vendored])),
  registeredPlugins: registered,
  strippedByInterop: strippedByInterop(),
  runtimeOnlyPaths: runtimeOnly,
  legendItemPaths: [...legendItemPaths].sort(),
  paths: [...paths].sort(),
};

fs.mkdirSync(path.dirname(outFile), { recursive: true });
fs.writeFileSync(outFile, JSON.stringify(output, null, 2) + '\n');

console.log(`wrote ${path.relative(repo, outFile)}`);
console.log(`  chart config paths from declarations : ${configPaths.size}`);
console.log(`  paths from vendored bundle defaults  : ${runtimePaths.size}`);
console.log(`  contributed by bundles alone         : ${runtimeOnly.length}`);
console.log(`  total                                : ${output.paths.length}`);
console.log(`  registered plugins                   : ${registered.join(', ')}`);
console.log(`  stripped by interop                  : ${output.strippedByInterop.join(', ')}`);
