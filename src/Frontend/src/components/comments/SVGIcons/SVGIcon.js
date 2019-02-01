import React, { Component } from 'react';

class SVGIcons extends Component {

	render() {
		return (
			<svg
				width={this.props.width}
				// style={style}
				height={this.props.height}
				// className={className}
				xmlns="http://www.w3.org/2000/svg"
				viewBox={this.getViewBox(this.props.name)}
				fill="none"
			>
				<path
					d={this.getPath(this.props.name)}
					fill="#333333"
					fill-opacity="0.71" />
			</svg>
		);
	}

	getViewBox = name => {
		switch (name) {
			case "bold":
				return "0 0 11 16";
			case "cursive":
				return "0 0 12 16";
			case "link":
				return "0 0 18 18";
			case "code":
				return "0 0 20 14";
			default:
				return "0 0 11 16";
		}
	};

	getPath = (name) => {
		switch (name) {
			case "bold":
				return "M8.6 7.83701C9.57 7.15421 10.25 6.03318 10.25 4.99369C10.25 2.69049 8.5 0.917236 6.25 0.917236H0V15.1848H7.04C9.13 15.1848 10.75 13.4523 10.75 11.3224C10.75 9.77333 9.89 8.44848 8.6 7.83701ZM3 3.46502H6C6.83 3.46502 7.5 4.14782 7.5 4.99369C7.5 5.83955 6.83 6.52236 6 6.52236H3V3.46502ZM6.5 12.637H3V9.57969H6.5C7.33 9.57969 8 10.2625 8 11.1084C8 11.9542 7.33 12.637 6.5 12.637Z";
			case "cursive":
				return "M4 0.917236V3.97457H6.21L2.79 12.1275H0V15.1848H8V12.1275H5.79L9.21 3.97457H12V0.917236H4Z";
			case "link":
				return 	"M8.6 7.83701C9.57 7.15421 10.25 6.03318 10.25 4.99369C10.25 2.69049 8.5 0.917236 6.25 0.917236H0V15.1848H7.04C9.13 15.1848 10.75 13.4523 10.75 11.3224C10.75 9.77333 9.89 8.44848 8.6 7.83701ZM3 3.46502H6C6.83 3.46502 7.5 4.14782 7.5 4.99369C7.5 5.83955 6.83 6.52236 6 6.52236H3V3.46502ZM6.5 12.637H3V9.57969H6.5C7.33 9.57969 8 10.2625 8 11.1084C8 11.9542 7.33 12.637 6.5 12.637Z";
			case "code":
				return "M7.4 11.7581L2.8 7.07013L7.4 2.3822L6 0.955444L0 7.07013L6 13.1848L7.4 11.7581ZM12.6 11.7581L17.2 7.07013L12.6 2.3822L14 0.955444L20 7.07013L14 13.1848L12.6 11.7581Z";
			default:
				return <path />;
		}
	};

// <svg width="18" height="18" viewBox="0 0 18 18"> link
// <svg width="12" height="16" viewBox="0 0 12 16"> cursive
// <svg width="11" height="16" viewBox="0 0 11 16"> bold
// <svg width="20" height="14" viewBox="0 0 20 14"> code
}

export default SVGIcons;
