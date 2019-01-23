import { Component } from "react";
import React from "react";
import styles from "./pages.less"

export function asPage(WrappedComponent) {
	return class extends Component {
		render() {
			return (
				<div className={styles.wrapper}>
					<div className={styles.contentWrapper}>
						<WrappedComponent {...this.props} />
					</div>
				</div>
			)
		}
	}
}