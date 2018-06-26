import React, { Component } from 'react';
import {Helmet} from "react-helmet";

/*
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
*/

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
        this.fetchContentFromServer(this.props.url);
    }

    componentWillReceiveProps(nextProps) {
        this.fetchContentFromServer(nextProps.url)
    }

    fetchContentFromServer(url) {
        fetch(this.BASE_URL + url, {credentials: 'include'})
            .then(response => response.text())
            .then(data => {
                let el = document.createElement('html');
                el.innerHTML = data;
                let head = el.getElementsByTagName('head')[0];
                let body = el.getElementsByTagName('body')[0];
                console.log(head);
                console.log(body);

                this.setState({
                    head: head.innerHTML,
                    body: body.innerHTML
                });

                /* Run scripts */
                (window.documentReadyFunctions || []).forEach(f => f());

                let embeddedScripts = Array.from(body.getElementsByTagName('script')).filter(s => !s.src).map(s => s.innerHTML);
                embeddedScripts.forEach(s => {
                    try {
                        console.log('Eval ', s);
// eslint-disable-next-line
                        eval(s)
                    } catch (e) {
                        console.error(e);
                    }
                });

                /* Scroll to top */
                window.scrollTo(0, 0);
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