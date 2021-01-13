import { FLAT_THEME, ThemeFactory, } from "ui";

export default ThemeFactory.create({
	btnBorderRadiusLarge: '8px',
	btnBorderRadiusMedium: '8px',
	btnBorderRadiusSmall: '8px',
}, FLAT_THEME);

export const textareaHidden = ThemeFactory.create({
	textareaBorderWidth: '0px',
	textareaBorderColorFocus: 'transparent',
	textareaWidth: '100px',
	textareaMinHeight: '20px',
}, FLAT_THEME);

//currently it only applies a dark background and white text in popup(tooltip), styles copied from dark theme
export var darkFlat = ThemeFactory.create({
	popupTextColor: '#fff',
	bgDefault: '#333333',
	btnBorderRadiusLarge: '8px',
	btnBorderRadiusMedium: '8px',
	btnBorderRadiusSmall: '8px',
}, FLAT_THEME);
