import { expect } from 'chai'
import path from 'path'

describe('sample', () => {
  let page

  before(async () => {
    page = await browser.newPage()
    const input = path.resolve('dist', 'ui', 'index.html')
    await page.goto(`file:///${input}`)
  })

  it('should look cool on screenshots', async () => {
    const output = path.resolve('dist', 'ui_test', 'test.jpg')
    await page.screenshot({ path: output })
  })

  it('should mount on root div', async () => {
    const root = '#root'
    await page.$(root, e => {
      expect(e).to.exist
      expect(e.innerHTML).to.not.be.empty
    })
  })

  it('should have correct colors', async () => {
    const numbers = '.number'
    const texts = '.text'
    const result = '#result'
    await Promise.all([
      page.$$(numbers, elements => {
        elements.forEach(e => expect(e.style.color).to.equal('#00008b'))
      }),
      page.$$(texts, elements => {
        elements.forEach(e => expect(e.style.color).to.equal('#333'))
      }),
      page.$(result, e => expect(e.style.color).to.equal('#006400')),
    ])
  })

  it('should count correctly', async () => {
    const x = '#x'
    const y = '#y'
    const result = '#result'
    await page.type(x, '2')
    await page.type(y, '3')
    await page.$(result, e => expect(e.innerText).to.equal('5'))
  })

  it('should work with keyboard', async () => {
    const x = '#x'
    const result = '#result'
    await page.focus(x)
    await page.keyboard.type('1')
    await page.keyboard.press('Tab')
    await page.keyboard.type('1')
    await page.$(result, e => expect(e.innerText).to.equal('2'))
  })
})
