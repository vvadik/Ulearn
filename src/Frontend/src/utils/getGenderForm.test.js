import getWordForm from './getWordForm';

describe('getWordForm', () => {
	test('для мужчины', () => {
		expect(getWordForm('была', 'был', 'male')).toBe('был');
	});
	test('для женщины', () => {
		expect(getWordForm('была', 'был', 'female')).toBe('была');
	});
	test('для пустой строки', () => {
		expect(getWordForm('была', 'был', '')).toBe('был');
	});
	test('для числа', () => {
		expect(getWordForm('была', 'был', 1)).toBe('был');
	});
	test('для числа', () => {
		expect(getWordForm('была', 'был', '1')).toBe('был');
	});
	test('для undefined', () => {
		expect(getWordForm('была', 'был', undefined)).toBe('был');
	});
	test('для null', () => {
		expect(getWordForm('была', 'был', null)).toBe('был');
	});
});
