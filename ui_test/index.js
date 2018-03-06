import '@babel/polyfill'
import util from 'util'
import fs from 'fs'
import path from 'path'
import Mocha from 'mocha'
import puppeteer from 'puppeteer'
const readDirAsync = util.promisify(fs.readdir)

const runTests = async () => {
  const testDir = __dirname
  const files = await readDirAsync(testDir)

  global.browser = await puppeteer.launch()

  const mocha = new Mocha({
    ui: 'bdd',
    reporter: 'json',
    globals: ['browser'],
  })

  files
    .filter(f => f.endsWith('.test.js'))
    .forEach(f => mocha.addFile(path.join(testDir, f)))

  return new Promise(resolve =>
    mocha.run(json => {
      resolve(json)
    })
  )
}

runTests()
  .then(console.log)
  .catch(console.error)
  .finally(global.browser.close)
