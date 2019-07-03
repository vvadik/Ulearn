import React, {Component} from 'react'
import PropTypes from "prop-types";
import styles from './guides.less'

class Guides extends Component {
	render() {
		return (
			<ol className={styles.guidesList}>
				{this.props.data.map((guide, index) =>
					<li className={styles.guidesElement} key={index}>
						{guide}
					</li>
				)}
			</ol>
		);
	}
}

Guides.propTypes = {
	data: PropTypes.array
};

export default Guides;