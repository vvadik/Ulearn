import { FLAT_THEME, ThemeFactory, } from "ui";

const roundButtons = {
	btnBorderRadiusLarge: '8px',
	btnBorderRadiusMedium: '8px',
	btnBorderRadiusSmall: '8px',
};

const roundSwitcher = {
	switcherButtonBorderRadiusLarge: '2px',
	switcherButtonPaddingXLarge: '30px',
	switcherLabelGapLarge: '24px',
};

export default ThemeFactory.create({
	...roundButtons,
	...roundSwitcher
}, FLAT_THEME);

export const textareaHidden = ThemeFactory.create({
	textareaBorderWidth: '0px',
	textareaBorderColorFocus: 'transparent',
	textareaWidth: '100px',
	textareaMinHeight: '20px',
}, FLAT_THEME);

//currently it only applies a dark background and white text in popup(tooltip), styles copied from dark theme
export const darkFlat = ThemeFactory.create({
	popupTextColor: '#fff',
	bgDefault: '#333333cc',
	btnBorderRadiusLarge: '8px',
	btnBorderRadiusMedium: '8px',
	btnBorderRadiusSmall: '8px',
}, FLAT_THEME);
