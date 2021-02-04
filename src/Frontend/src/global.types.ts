declare global {
	interface Window {
		config: {
			api: {
				endpoint: string
			}
		},
		reactHistory: unknown,
	}
}

export {};
