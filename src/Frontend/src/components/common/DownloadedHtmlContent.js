import React, { Component } from 'react';
import { Helmet } from "react-helmet";
import Loader from "@skbkontur/react-ui/Loader"


class DownloadedHtmlContent extends Component {
    BASE_URL = '';

    constructor(props) {
        super(props);

        this.state = {
            loading: true,
            body: '',
            meta: {}
        };
    }

    componentDidMount() {
        this.fetchContentFromServer(this.props.url);
    }

    componentWillReceiveProps(nextProps) {
        this.fetchContentFromServer(nextProps.url);

        /* Remove bootstrap's modal backdrop */
        let body = document.getElementsByTagName('body')[0];
        body.classList.remove('modal-open');
        let backdrop = body.getElementsByClassName('modal-backdrop')[0];
        if (backdrop)
            backdrop.remove();
    }

    fetchContentFromServer(url) {
        this.setState(s => {
            s.loading = true;
        });
        fetch(this.BASE_URL + url, {credentials: 'include'})
            .then(response => response.text())
            .then(data => {
                let el = document.createElement('html');
                el.innerHTML = data;
                let body = el.getElementsByTagName('body')[0];

                this.setState({
                    loading: false,
                    body: body.innerHTML
                });

                /* Run scripts */
                (window.documentReadyFunctions || []).forEach(f => f());

                let embeddedScripts = Array.from(body.getElementsByTagName('script')).filter(s => !s.src).map(s => s.innerHTML);
                embeddedScripts.forEach(s => {
                    try {
// eslint-disable-next-line
                        eval(s)
                    } catch (e) {
                        console.error(e);
                    }
                });

                /* Scroll to top */
                window.scrollTo(0, 0);

                let meta = window.meta || {
                    title : 'Ulearn',
                    description: 'Интерактивные учебные онлайн-курсы по программированию',
                    keywords: '',
                    imageUrl: '',
                };
                this.setState(s => {
                    s.loading = false;
                    s.meta = meta
                });
            });
    }

    render() {
        if (this.state.loading) {
            return (
                <Loader type="big" active>
                    { this.getContent() }
                </Loader>
            )
        }

        return this.getContent();
    }

    getContent() {
        let meta = this.state.meta;
        return (
            <div>
                <Meta meta={meta} />
                <div dangerouslySetInnerHTML={{__html: this.state.body}}/>
            </div>
        )
    }
}

class Meta extends Component {
    render() {
        let meta = this.props.meta;
        return (
            <Helmet>
                <title>{ meta.title }</title>
                <meta name="title" content={ meta.title }/>
                <meta property="og:title" content={ meta.title }/>
                <meta property="og:image" content={ meta.imageUrl }/>
                <meta property="og:image:alt" content={ meta.description }/>
                <meta property="og:description" content={ meta.description }/>
                <meta property="og:locale" content="ru_RU"/>
                <meta property="og:site_name" content="Ulearn"/>
                <meta name="description" content={ meta.description }/>
                <meta name="keywords" content={ meta.keywords }/>
                <link rel="image_src" href={ meta.imageUrl }/>
            </Helmet>
        )
    }
}

export default DownloadedHtmlContent;