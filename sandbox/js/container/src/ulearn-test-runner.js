const path = require('path')
const fs = require('fs')
const util = require('util')
const { exec } = require('child_process')
const Mocha = require('mocha')

const execAsync = util.promisify(exec)
const readDirAsync = util.promisify(fs.readdir)

const testDir = path.resolve(__dirname, 'src')
const hasUnitTests = fs.readdirSync(testDir).filter(f => f.endsWith('.test.js'))

const buildTests = () => {
  const commands = []

  if (hasUnitTests) {
    commands.push(execAsync('yarn build-unit-tests'))
  }

  return Promise.all(commands)
}

const pickTestFiles = async testDir =>
  (await readDirAsync(testDir)).filter(f => f.endsWith('.test.js'))

const runUnitTests = async () => {
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
  if (hasUnitTests) {
    await runUnitTests()
  }
}

const cleanup = () => global.browser && global.browser.close()

buildTests()
  .then(runTests)
  .catch(console.error)
  .then(cleanup, cleanup)
