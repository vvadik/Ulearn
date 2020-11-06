import { FLAT_THEME, ThemeFactory, } from "ui";

export default ThemeFactory.create({
	btnBorderRadius: '8px',
	btnSmallBorderRadius: '8px',
}, FLAT_THEME);

export const textareaHidden = ThemeFactory.create({
	textareaBorderWidth: '0px',
	textareaBorderColorFocus: 'transparent',
	textareaWidth: '100px',
	textareaMinHeight: '20px',
}, FLAT_THEME);
