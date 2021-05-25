import { VisualizerStatus } from "./VusualizerStatus";

const texts = {
	visualizer: "Визуализатор",

	stepsCounter: {
		currentStepNumber: (currentStep: number, totalSteps: number) : string =>
			`Шаг ${currentStep === 0 ? currentStep : currentStep + 1} из ${ totalSteps }`,
		status: (status: VisualizerStatus) : string | null => {
			if (status === VisualizerStatus.Ok) {
				return null;
			}
			if (status === VisualizerStatus.Return) {
				return "Завершение функции";
			}
			if (status === VisualizerStatus.Error) {
				return "Произошла ошибка";
			}
			return null;
		}
	},

	dataArea: {
		inputData: "Входные данные",
		outputData: "Выходные данные",
	},

	controls: {
		run: "Запустить",
		back: "Назад",
		next: "Далее"
	},
};

export default texts;
