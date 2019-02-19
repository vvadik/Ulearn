import React from "react";
import PropTypes from "prop-types";
import Button from "@skbkontur/react-ui/components/Button/Button";

function Action(props) {
	const { commentId, sending, action } = props;
	let text = '';

	return (
		<Button
			use="primary"
			size="medium"
			type="submit"
			loading={sending}>
			{ action === 'edit' ? 'Cохранить' : 'Оставить комментарий' }
		</Button>
	);
}

Action.propTypes = {
	commentId: PropTypes.number,
	sending: PropTypes.bool,
};

export default Action;