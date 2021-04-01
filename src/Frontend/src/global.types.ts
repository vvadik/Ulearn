declare global {
	interface Window {
		config: {
			api: {
				endpoint: string
			}
		},
		reactHistory: unknown,
		//legacy scripts
		documentReadyFunctions: (() => void)[],
		//scripts used by cshtml scripts
		loginForContinue: () => void,
		diffHtml: () => void,
		likeSolution: () => void,
		ToggleSystemRoleOrAccess: () => void,
		ToggleButtonClass: () => void,
		ToggleDropDownClass: () => void,
		openPopup: () => void,
		closePopup: () => void,

		//yandex metrika used in legacy cshmtl
		ym: unknown,
	}
}

export {};
