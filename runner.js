const path = require('path')
const fs = require('fs')
const util = require('util')
const { exec } = require('child_process')

const execAsync = util.promisify(exec)
const readFileAsync = util.promisify(fs.readFile)
const writeFileAsync = util.promisify(fs.writeFile)

const runTests = () =>
  Promise.all([
    execAsync('yarn build-ui && yarn run-ui-tests'),
    execAsync('yarn run-unit-tests'),
  ])

const report = async () => {
  const uiPath = path.join(__dirname, 'dist', 'ui-tests', 'ui-tests-result.json')
  const unitPath = path.join(
    __dirname,
    'dist',
    'unit-tests',
    'unit-tests-result.json'
  )

  const [ui, unit] = (await Promise.all([
    readFileAsync(uiPath),
    readFileAsync(unitPath),
  ])).map(buffer => JSON.parse(buffer.toString()))

  const result = {
    ui,
    unit,
  }

  const output = path.join(__dirname, 'output', 'result.json')
  await writeFileAsync(output, JSON.stringify(result))

  console.log('test runner finished')
}

const fail = err => {
  console.error('test runner failed')
  console.error(err)
  process.exit(1)
}

runTests()
  .then(report)
  .catch(fail)
