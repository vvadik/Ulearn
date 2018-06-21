import React, { Component } from 'react';
import {Helmet} from "react-helmet";

function loadScripts(sources){
    let loadScriptAsync = function (src) {
        let script = document.createElement("script");
        script.src = src;
        script.async = true;

        let head = document.getElementsByTagName("head")[0];
        (head || document.body).appendChild(script);
    };

    sources.map(s => loadScriptAsync(s));
}

class DownloadedHtmlContent extends Component {
    BASE_URL = '';

    constructor(props) {
        super(props);

        this.state = {
            head: '',
            body: '',
        };
    }

    componentDidMount() {
        fetch(this.BASE_URL + this.props.url, { credentials: 'include' })
            .then(response => response.text())
            .then(data => {
                let el = document.createElement('html');
                el.innerHTML = data;
                let head = el.getElementsByTagName('head')[0];
                let body = el.getElementsByTagName('body')[0];
                console.log(head);
                console.log(body);
                // let scripts = Array.from(body.getElementsByTagName('script')).map(s => s.src);
                // console.log(scripts);

                this.setState({
                    head: head.innerHTML,
                    body: body.innerHTML
                });

                // loadScripts(scripts);
                loadScripts([
                    "/scripts.bundle.js",
                    "https://cdnjs.cloudflare.com/ajax/libs/diff_match_patch/20121119/diff_match_patch.js"
                ]);
            });
    }

    render() {

        return (
            <div>
                <Helmet>
                    <meta charSet="utf-8" />
                    <title>Ulearn</title>
                </Helmet>
                <div dangerouslySetInnerHTML={{__html: this.state.body}}/>
            </div>
        )
    }
}

export default DownloadedHtmlContent;