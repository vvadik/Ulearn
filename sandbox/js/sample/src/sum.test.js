import { expect } from "chai";
import sum from "./sum";

describe("sum", () => {
  it("should sum 2 and 2", () => {
    expect(sum(2, 2)).to.equal(4);
  });

  it('should sum "2" and "2"', () => {
    expect(sum("2", "2")).to.equal("22");
  });

  it("might fail", () => {
    expect(true).to.equal(false);
  });
});
