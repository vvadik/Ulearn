import React, { Component } from "react";
import { YMInitializer } from "react-yandex-metrika";
import ym from 'react-yandex-metrika';
import PropTypes from "prop-types";
import { withRouter } from "react-router-dom";

class YandexMetrika extends Component {
	componentDidUpdate() {
		ym('hit', this.props.location.pathname + this.props.location.search);
	}

	render() {
		return (
			<div>
				<YMInitializer accounts={[25997251]} options={{
					clickmap: true,
					trackLinks: true,
					accurateTrackBounce: true,
					webvisor: true,
				}} />
			</div>
		);
	}

	static propTypes = {
		location: PropTypes.object.isRequired,
	}
}

export default withRouter(YandexMetrika);