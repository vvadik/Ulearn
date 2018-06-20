import { expect } from 'chai'

describe('mocha', () => {
  it('should have access to global puppeteer', () => {
    expect(browser).to.exist
  })

  it('should be headless', async () => {
    const version = await browser.version()
    expect(version.startsWith('HeadlessChrome')).to.be.true
  })
})
