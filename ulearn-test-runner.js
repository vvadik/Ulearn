const path = require('path')
const fs = require('fs')
const util = require('util')
const { exec } = require('child_process')
const Mocha = require('mocha')
const puppeteer = require('puppeteer')

const execAsync = util.promisify(exec)
const readDirAsync = util.promisify(fs.readdir)

const hasUiTests = fs.existsSync(path.resolve(__dirname, 'tests', 'ui'))
const hasUnitTests = fs.existsSync(path.resolve(__dirname, 'tests', 'unit'))

const buildTests = () => {
  const commands = []

  if (hasUiTests) {
    commands.push(execAsync('npm run build-ui && npm run build-ui-tests'))
  }

  if (hasUnitTests) {
    commands.push(execAsync('npm run build-unit-tests'))
  }

  return Promise.all(commands)
}

const pickTestFiles = async testDir =>
  (await readDirAsync(testDir)).filter(f => f.endsWith('.test.js'))

const runUiTests = async () => {
  const testDir = path.resolve(__dirname, 'dist', 'tests', 'ui')
  const testFiles = await pickTestFiles(testDir)

  if (testFiles.length === 0) {
    return
  }

  global.browser = await puppeteer.launch({
    args: ['--headless', '--disable-gpu', '--no-sandbox'],
  })

  const mocha = new Mocha({
    ui: 'bdd',
    globals: ['browser'],
  })

  testFiles.forEach(f => mocha.addFile(path.join(testDir, f)))

  return new Promise(resolve => {
    mocha.run(resolve)
  })
}

const runUnitTests = async () => {
  const testDir = path.resolve(__dirname, 'dist', 'tests', 'unit')
  const testFiles = await pickTestFiles(testDir)

  if (testFiles.length === 0) {
    return
  }

  const mocha = new Mocha({
    ui: 'bdd',
  })

  testFiles.forEach(f => mocha.addFile(path.join(testDir, f)))

  return new Promise(resolve => {
    mocha.run(resolve)
  })
}

const runTests = async () => {
  if (hasUiTests) {
    await runUiTests()
  }

  if (hasUnitTests) {
    await runUnitTests()
  }
}

const cleanup = () => global.browser && global.browser.close()

buildTests()
  .then(runTests)
  .catch(console.error)
  .then(cleanup, cleanup)
