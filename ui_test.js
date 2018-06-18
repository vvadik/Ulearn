const util = require('util')
const fs = require('fs')
const path = require('path')
const Mocha = require('mocha')
const puppeteer = require('puppeteer')

const readDirAsync = util.promisify(fs.readdir)
const writeFileAsync = util.promisify(fs.writeFile)

const testDir = path.resolve(__dirname, 'dist', 'ui_test')

const runTests = async () => {
  const files = await readDirAsync(testDir)

  global.browser = await puppeteer.launch({
    executablePath: '/usr/bin/chromium-browser',
    args: ['--disable-dev-shm-usage', '--headless', '--disable-gpu', '--no-sandbox'], // TODO: remove on Chrome 65+
  })

  const mocha = new Mocha({
    ui: 'bdd',
    reporter: 'json',
    globals: ['browser'],
  })

  files
    .filter(f => f.endsWith('.test.js'))
    .forEach(f => mocha.addFile(path.join(testDir, f)))

  const write = process.stdout.write
  process.stdout.write = () => {}

  return new Promise(resolve => {
    const runner = mocha.run(() => {
      process.stdout.write = write
      resolve(JSON.stringify(runner.testResults, null, 2))
    })
  })
}

runTests()
  .then(async res => {
    console.log('ui test completed')
    await writeFileAsync(path.join(testDir, 'ui-test-result.json'), res)
  })
  .catch(err => {
    console.error('ui test failed')
    console.error(err)
    process.exit(1)
  })
  .then(() => global.browser.close(), () => global.browser.close())
