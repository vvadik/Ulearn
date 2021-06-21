import React from "react";
import { CourseAccessType, CourseRoleType, SystemAccessType } from "../consts/accessType";
import { UserInfo } from "../utils/courseRoles";
import { getMockedUser } from "../components/comments/storiesData";

export const mock = (): unknown => ({});

interface Props<T> {
	args: T;
	childrenBuilder: (args: T) => React.ReactElement<T>;
	enableLogger?: boolean;
}

interface State {
	version: number;
}

/*const Template: Story<Test> = args => {
	return <StoryUpdater args={ args } childrenBuilder={ (args) => <TestComponent { ...args }/> }/>;
};*/

//Special class which updating all fields within args function calls. Providing args as this inside functions
export class StoryUpdater<T> extends React.Component<Props<T>, State> {
	constructor(props: Props<T>) {
		super(props);
		const { args, enableLogger, } = this.props;

		for (const key in args) {
			if(enableLogger) {
				console.log(`UPDATER: checking ${ key }`);
			}
			if(typeof args[key] === 'function') {
				if(enableLogger) {
					console.log(`UPDATER: binding ${ key }`);
				}

				const base = args[key];

				// eslint-disable-next-line @typescript-eslint/ban-ts-comment
				// @ts-ignore
				args[key] = ((...funcArgs: unknown[]) => {
					this.setState({ ...this.state, version: this.state.version + 1, });

					if(enableLogger) {
						console.log(`UPDATER: get new version ${ this.state.version }`);
					}

					// eslint-disable-next-line @typescript-eslint/ban-ts-comment
					// @ts-ignore
					return base.bind(args)(...funcArgs);
				});
			}
		}

		this.state = {
			version: 0,
		};
	}

	render(): React.ReactElement {
		const { childrenBuilder, args, } = this.props;

		return childrenBuilder(args);
	}
}

export const student: UserInfo = getMockedUser({
	isAuthenticated: true,
	id: "1",
	isSystemAdministrator: false,
	courseRole: CourseRoleType.student,
	visibleName: "Иван Иванов",
	lastName: 'Иванов',
	firstName: 'Иван',
	email: 'student mail',
	login: 'student@urfu.ru',
});
export const unAuthUser: UserInfo = getMockedUser({
	isAuthenticated: false,
});
export const instructor: UserInfo = getMockedUser({
	...student,
	courseRole: CourseRoleType.instructor,
});
export const courseAdmin: UserInfo = getMockedUser({
	...student,
	courseRole: CourseRoleType.courseAdmin,
});
export const sysAdmin: UserInfo = getMockedUser({
	isAuthenticated: true,
	id: "1",
	isSystemAdministrator: true,
	courseRole: CourseRoleType.student,
	visibleName: "Иван Иванов",
	lastName: 'Иванов',
	firstName: 'Иван',
	email: 'admin@ulearn.me',
	login: 'admin',
});
export const avatarUrl = 'https://staff.skbkontur.ru/content/images/default-user-woman.png';
export const accessesToSeeProfiles: SystemAccessType[] = [SystemAccessType.viewAllProfiles];
export const courseAccessesToEditComments: CourseAccessType[] = [CourseAccessType.editPinAndRemoveComments];
export const courseAccessesToViewSubmissions: CourseAccessType[] = [CourseAccessType.viewAllStudentsSubmissions];
