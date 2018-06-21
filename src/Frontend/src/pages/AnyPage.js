import React, { Component } from 'react';
import DownloadedHtmlContent from '../components/common/DownloadedHtmlContent'

class AnyPage extends Component {
    render() {
        let url = window.location.pathname;
        console.log(url);
        if (url === "" || url === "/") {
            url = "/CourseList"
        }

        return (
            <DownloadedHtmlContent url={url}/>
        )
    }
}

export default AnyPage;