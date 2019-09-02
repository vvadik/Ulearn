import { buildQuery } from './index';

describe('buildQuery should', () => {
	test('be correct for empty params', () => {
		const result = buildQuery();

		expect(result).toBeNull();
	});

	test('be correct for 1 parameter', () => {
		const params = { parameter1: true };

		const result = buildQuery(params);

		expect(result).toBe('?parameter1=true');
	});


	test.each([
		[{ a: 'a', b: 2, c: 2.5 }, '?a=a&b=2&c=2.5'],
		[{ a: 'a', b: 'b', c: 'c' }, '?a=a&b=b&c=c'],
		[{ a: 'parameter with spaces' }, '?a=parameter%20with%20spaces'],
		[{ a: true, b: false, NaN: NaN, null: null }, '?a=true&b=false&NaN=NaN&null=null'],
		[{ undefined: undefined }, null],
		[{ a: undefined, b: undefined, c: undefined }, null],
		[{ array: ['1', 2, true, null, NaN, undefined] }, '?array=1%2C2%2Ctrue%2C%2CNaN%2C'],
	])('%o params to be %s',
		(params, expectedResult) => {
			const result = buildQuery(params);

			expect(result).toEqual(expectedResult);
		});
});