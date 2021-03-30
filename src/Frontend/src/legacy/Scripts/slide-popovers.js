window.documentReadyFunctions = window.documentReadyFunctions || [];

import { createPopper } from '@popperjs/core';

window.documentReadyFunctions.push(function () {
	const elem = $('.popover-trigger');
	if(elem)
		createPopper(
			elem,
			{ html: true }
		);
});
