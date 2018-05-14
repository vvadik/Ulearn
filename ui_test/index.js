import '@babel/polyfill'
import util from 'util'
import fs from 'fs'
import path from 'path'
import Mocha from 'mocha'
import puppeteer from 'puppeteer'

const readDirAsync = util.promisify(fs.readdir)
const writeFileAsync = util.promisify(fs.writeFile)

const runTests = async () => {
  const testDir = __dirname
  const files = await readDirAsync(testDir)

  global.browser = await puppeteer.launch({
    executablePath: '/usr/bin/chromium-browser',
    args: ['--disable-dev-shm-usage'],
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
  const outputBuffer = []
  process.stdout.write = str => {
    outputBuffer.push(str)
  }

  return new Promise(resolve =>
    mocha.run(() => {
      process.stdout.write = write
      resolve(outputBuffer.join())
    })
  )
}

runTests()
  .then(async res => {
    console.log('ui test completed')
    await writeFileAsync(path.join(__dirname, 'ui-test-result.json'), res)
  })
  .catch(err => {
    console.error('ui test failed')
    console.error(err)
    process.exit(1)
  })
  .finally(() => global.browser.close())
