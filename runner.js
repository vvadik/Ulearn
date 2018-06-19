const path = require('path')
const fs = require('fs')
const util = require('util')
const { exec } = require('child_process')

const execAsync = util.promisify(exec)
const readFileAsync = util.promisify(fs.readFile)
const writeFileAsync = util.promisify(fs.writeFile)

const hasUiTests = fs.existsSync(path.resolve(__dirname, 'ui-tests'))
const hasUnitTests = fs.existsSync(path.resolve(__dirname, 'unit-tests'))

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
  const commands = {}

  if (hasUiTests) {
    const uiPath = path.join(
      __dirname,
      'dist',
      'ui-tests',
      'ui-tests-result.json'
    )
    commands.ui = read(uiPath)
  }

  if (hasUnitTests) {
    const unitPath = path.join(
      __dirname,
      'dist',
      'unit-tests',
      'unit-tests-result.json'
    )
    commands.unit = read(unitPath)
  }

  await Promise.all(Object.values(commands))

  const result = {
    ui: (await commands.ui) || {},
    unit: (await commands.unit) || {},
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
