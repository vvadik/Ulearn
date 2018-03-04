import util from 'util'
import fs from 'fs'
import path from 'path'
import Mocha from 'mocha'

const readDirAsync = util.promisify(fs.readdir)

const runTests = async () => {
  const testDir = __dirname
  const files = await readDirAsync(testDir)

  const mocha = new Mocha({
    ui: 'bdd',
    reporter: 'json',
  })

  files
    .filter(f => f.endsWith('.test.js'))
    .forEach(f => mocha.addFile(path.join(testDir, f)))

  mocha.run(json => {
    console.log(json)
  })
}

runTests().catch()
