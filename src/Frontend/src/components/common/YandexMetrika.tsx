import React, { Component } from "react";
import { YMInitializer } from "react-yandex-metrika";
import ym from 'react-yandex-metrika';
import { RouteComponentProps, withRouter } from "react-router-dom";

class YandexMetrika extends Component<RouteComponentProps> {
	componentDidUpdate() {
		const { location, } = this.props;
		ym('hit', location.pathname + location.search);
	}

	render() {
		return (
			<div>
				<YMInitializer accounts={ [25997251] } options={ {
					clickmap: true,
					trackLinks: true,
					accurateTrackBounce: true,
					webvisor: true,
				} }/>
			</div>
		);
	}
}

export default withRouter(YandexMetrika);
