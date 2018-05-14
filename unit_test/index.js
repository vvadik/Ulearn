import util from 'util'
import fs from 'fs'
import path from 'path'
import Mocha from 'mocha'

const readDirAsync = util.promisify(fs.readdir)
const writeFileAsync = util.promisify(fs.writeFile)

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
    console.log('unit test completed')
    await writeFileAsync(path.join(__dirname, 'unit-test-result.json'), res)
  })
  .catch(err => {
    console.error('unit test failed')
    console.error(err)
    process.exit(1)
  })
