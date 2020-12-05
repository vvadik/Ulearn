export enum Gender
{
	Male = 'male',
	Female = 'female'
}

export interface ShortUserInfo {
	id: string
	login: string | undefined
	email: string | undefined
	firstName: string
	lastName: string
	visibleName: string
	avatarUrl: string
	gender: Gender | null
}
