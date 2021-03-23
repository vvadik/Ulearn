import React from "react";
import { EyeClosed, Ok, Pin, People } from "icons";
import { TooltipMenu, MenuItem } from "ui";

import { Comment, ShortGroupInfo } from "src/models/comments";

import styles from "./Marks.less";


interface MarksProps {
	authorGroups: ShortGroupInfo[];
	courseId: string;
	comment: Comment;
}

export default function Marks({ courseId, comment, authorGroups }: MarksProps): React.ReactElement {
	const windowUrl = `${ window.location.origin }/${ courseId }`;
	return (
		<>
			{ !comment.isApproved && <HiddenMark/> }
			{ comment.isCorrectAnswer && <CorrectAnswerMark/> }
			{ comment.isPinnedToTop && <PinnedToTopMark/> }
			{ authorGroups && <GroupMark url={ windowUrl } groups={ authorGroups }/> }
		</>
	);
}

const HiddenMark = () => (
	<div className={ `${ styles.mark } ${ styles.approvedComment }` }>
		<EyeClosed size={ 15 }/>
		<span className={ `${ styles.text } ${ styles.visibleOnDesktopAndTablet }` }>
			Скрыт
		</span>
	</div>
);

const CorrectAnswerMark = () => (
	<div className={ `${ styles.mark } ${ styles.correctAnswer }` }>
		<Ok size={ 15 }/>
		<span className={ `${ styles.text } ${ styles.visibleOnDesktopAndTablet }` }>
			Правильный&nbsp;ответ
		</span>
	</div>
);

const PinnedToTopMark = () => (
	<div className={ `${ styles.mark } ${ styles.pinnedToTop }` }>
		<Pin size={ 15 }/>
		<span className={ `${ styles.text } ${ styles.visibleOnDesktopAndTablet }` }>
			Закреплен
		</span>
	</div>
);

interface GroupProps {
	url: string;
	groups: ShortGroupInfo[];
}

export function GroupMark({ url, groups }: GroupProps): React.ReactElement {
	const groupsNumber = groups.length;

	return (
		<>
			<div className={ styles.visibleOnDesktopAndTablet }>
				<div className={ styles.groupList }>
					{ groupsNumber < 3 ?
						groups.map(group =>
							<div key={ group.id }
								 className={ `${ styles.mark } ${ styles.group } ${ group.isArchived && styles.archiveGroup }` }>
								<People size={ 15 }/>
								<a href={ group.apiUrl && `${ url }${ group.apiUrl }` }
								   className={ `${ styles.text } ${ styles.groupName }` }>
									{ group.name }
								</a>
							</div>) :
						<GroupsMenu url={ url } groups={ groups }/> }
				</div>
			</div>
			<div className={ styles.visibleOnPhone }>
				<GroupsMenu url={ url } groups={ groups }/>
			</div>
		</>
	);
}

const GroupsMenu = ({ url, groups }: GroupProps) => (
	<TooltipMenu
		menuWidth="150px"
		positions={ ["bottom right"] }
		caption={
			<div className={ styles.groupMarkOnPhone }>
				<People color="#fff" size={ 15 }/>
				<span className={ `${ styles.text } ${ styles.visibleOnDesktopAndTablet }` }>
					Группы
				</span>
			</div> }>
		<>
			{ groups.map(group => !group.isArchived &&
				<MenuItem
					key={ group.id }
					href={ group.apiUrl && `${ url }${ group.apiUrl }` }>
					{ group.name }
				</MenuItem>) }
			{ groups.map(group => group.isArchived &&
				<MenuItem
					key={ group.id }
					href={ group.apiUrl && `${ url }${ group.apiUrl }` }>
					{ group.name }
				</MenuItem>) }
		</>
	</TooltipMenu>
);
