import { Component } from "react";
import React from "react";

/*
Just an example for inserting react component inside legacy layout (cshtml+jquery).
Feel free to remove this class and write a new one.
*/
class Comment extends Component {
	render() {
		return (
			<div>
				Hello, it's a comment. Props are {this.props.first} and {this.props.second}
			</div>
		);
	}
}

export default Comment;