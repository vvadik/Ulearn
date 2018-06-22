const util = require('util')
const fs = require('fs')
const path = require('path')
const Mocha = require('mocha')
const puppeteer = require('puppeteer')

const readDirAsync = util.promisify(fs.readdir)
const writeFileAsync = util.promisify(fs.writeFile)

const testDir = path.resolve(__dirname, 'dist', 'tests', 'ui')

const runTests = async () => {
  const testFiles = (await readDirAsync(testDir)).filter(f =>
    f.endsWith('.test.js')
  )

  if (testFiles.length === 0) {
    return JSON.stringify({})
  }

  global.browser = await puppeteer.launch({
    executablePath: '/usr/bin/chromium-browser',
    args: [
      '--disable-dev-shm-usage',
      '--headless',
      '--disable-gpu',
      '--no-sandbox',
    ], // TODO: remove on Chrome 65+
  })

  const mocha = new Mocha({
    ui: 'bdd',
    reporter: 'json',
    globals: ['browser'],
  })

  testFiles.forEach(f => mocha.addFile(path.join(testDir, f)))

  const write = process.stdout.write
  process.stdout.write = () => {}

  return new Promise(resolve => {
    const runner = mocha.run(() => {
      process.stdout.write = write
      resolve(JSON.stringify(runner.testResults, null, 2))
    })
  })
}

const cleanup = () => global.browser && global.browser.close()

runTests()
  .then(async res => {
    console.log('ui tests completed')
    await writeFileAsync(path.join(testDir, 'ui-tests-result.json'), res)
  })
  .catch(err => {
    console.error('ui tests failed')
    console.error(err)
    process.exit(1)
  })
  .then(cleanup, cleanup)
