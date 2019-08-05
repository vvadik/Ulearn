import React, {Component} from 'react';
import Modal from '@skbkontur/react-ui/components/Modal/Modal';
import Button from '@skbkontur/react-ui/components/Button/Button';

import Textarea from '@skbkontur/react-ui/components/Textarea/Textarea';

import styles from './ToggleRolesModal.less';

class ToggleRolesModal extends Component {
	constructor(props) {
		super(props);

		this.state = {
			comment: null,
			modalOpened: null,
			error: null,
		};
	}

	componentDidMount() {
		this.root = document.querySelector('.react-rendered');

		this.observer = new MutationObserver((mutations) => {
			mutations.forEach((mutation) => {
				this.setState({
					modalOpened: this.root.getAttribute(mutation.attributeName) === 'true',
				});
			});
		});

		const config = {attributes: true};

		this.observer.observe(this.root, config);
	}

	componentDidUpdate(prevProps, prevState, snapshot) {
		if (prevState.modalOpened !== this.state.modalOpened) {
			this.setState({
				comment: null,
				error: null,
			})
		}
	}

	componentWillUnmount() {
		this.observer.disconnect();
	}

	render() {
		const {modalOpened, error} = this.state;

		return (
			<div>
				{modalOpened &&
				<Modal onClose={this.onClose}>
					<Modal.Header>
						Оставьте комментарий
					</Modal.Header>
					<Modal.Body>
						<p className={styles.errorMessage}> {error && error} </p>
						<Textarea
							width='100%'
							onChange={this.onTextareaChange}
							autoResize
							placeholder="Укажите причину назначения прав"
							error={error !== null}
						/>
						<Button use='primary' onClick={() => {
							this.onSubmit()
						}}>
							Сохранить
						</Button>
					</Modal.Body>
				</Modal>}
			</div>
		)
	}

	onTextareaChange = (event, value) => {
		this.setState({comment: value, error: null});
	};

	onClose = () => this.root.setAttribute('modalOpened', false)

	onSubmit = () => {
		const {comment} = this.state;

		const regex = /(?!\b\s+\b)\s+/g;

		const trimmedComment = comment
			? comment.replace(regex, ' ')
			: '';


		if (!trimmedComment || trimmedComment.length=== 0 || trimmedComment ===' ' ) {
			this.setState({
				error: 'Комментарий не может быть пустым',
			});
			return;
		}

		window.toggleRoleOrCourseAccess(trimmedComment);

		this.onClose();
	}
}

export default ToggleRolesModal;