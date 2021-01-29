export enum Gender {
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

export interface UsersSearchResponse {
	users: FoundUserResponse[],
}

export interface FoundUserResponse {
	user: ShortUserInfo,
	fields: SearchField[],
}

export enum SearchField {
	UserId,
	Login,
	Name,
	Email,
	SocialLogin,
}
