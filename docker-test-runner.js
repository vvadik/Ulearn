const path = require('path')
const fs = require('fs')
const util = require('util')
const { exec } = require('child_process')

const execAsync = util.promisify(exec)
const readFileAsync = util.promisify(fs.readFile)
const writeFileAsync = util.promisify(fs.writeFile)

const hasUiTests = fs.existsSync(path.resolve(__dirname, 'tests', 'ui'))
const hasUnitTests = fs.existsSync(path.resolve(__dirname, 'tests', 'unit'))

const runTests = () => {
  const commands = []

  if (hasUiTests) {
    commands.push(
      execAsync('yarn build-ui && yarn build-ui-tests && node ui-tests.js')
    )
  }

  if (hasUnitTests) {
    commands.push(execAsync('yarn build-unit-tests && node unit-tests.js'))
  }

  return Promise.all(commands)
}

const read = resultPath =>
  readFileAsync(resultPath)
    .then(parse)
    .catch(fail)

const parse = buffer => JSON.parse(buffer.toString())

const report = async () => {
  const uiCommand = hasUiTests
    ? read(path.join(__dirname, 'dist', 'tests', 'ui', 'ui-tests-result.json'))
    : null

  const unitCommand = hasUnitTests
    ? read(path.join(__dirname, 'dist', 'tests', 'unit', 'unit-tests-result.json'))
    : null

  const result = {
    ui: (await uiCommand) || {},
    unit: (await unitCommand) || {},
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
