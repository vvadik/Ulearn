import React, { Component, PureComponent } from 'react';
import { Helmet } from "react-helmet";
import Loader from "@skbkontur/react-ui/Loader"
import * as PropTypes from "prop-types";
import { saveAs } from "file-saver";


function getUrlParts(url) {
    let a = document.createElement('a');
    a.href = url;

    return {
        href: a.href,
        host: a.host,
        hostname: a.hostname,
        port: a.port,
        pathname: a.pathname,
        protocol: a.protocol,
        hash: a.hash,
        search: a.search
    };
}

function safeEval(code) {
    try {
// eslint-disable-next-line
        eval(code)
    } catch (e) {
        console.error(e);
    }
}

class DownloadedHtmlContent extends Component {
    BASE_URL = '';

    constructor(props) {
        super(props);

        this.state = {
            loading: true,
            body: '',
            meta: {},
            links: this.addUlearnBundleCssIfNeeded('', [])
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

    getCurrentBodyContent() {
        let body = document.getElementsByTagName('body')[0];
        return body.innerHTML;
    }

    addUlearnBundleCssIfNeeded(url, links) {
        if (url.startsWith('/Certificate/') || url.startsWith('/elmah'))
            return links;
        links.push({
            rel: 'stylesheet',
            type: '',
            href: '/ulearn.bundle.css'
        });
        return links;
    }

    fetchContentFromServer(url) {
        const self = this;
        fetch(this.BASE_URL + url, {credentials: 'include'})
            .then(response => {
                if (response.redirected) {
                    let url = getUrlParts(response.url);
                    this.context.router.history.replace(url.pathname);
                }
                if (response.headers.has('Content-Disposition')) {
                    let contentDisposition = response.headers.get('Content-Disposition');
                    if (contentDisposition.indexOf('attachment') !== -1) {
                        const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                        let matches = filenameRegex.exec(contentDisposition);
                        if (matches != null && matches[1]) {
                            let filename = matches[1].replace(/['"]/g, '');
                            response.blob().then(b => saveAs(b, filename, false));
                            return Promise.resolve(undefined)
                        }
                    }
                }
                this.setState(s => {
                    s.loading = true;
                    return s;
                });
                return response.text();
            })
            .then(data => {
                if (data === undefined) {
                    return;
                }

                let el = document.createElement('html');
                el.innerHTML = data;
                let head = el.getElementsByTagName('head')[0];
                let body = el.getElementsByTagName('body')[0];

                let links = Array.from(head.getElementsByTagName('link'));
                let titles = head.getElementsByTagName('title');

                links = this.addUlearnBundleCssIfNeeded(url, links);

                this.setState({
                    loading: false,
                    body: body.innerHTML,
                    links: links
                });

                /* Run scripts */
                (window.documentReadyFunctions || []).forEach(f => f());

                window.meta = undefined;
                let allScriptTags = Array.from(body.getElementsByTagName('script'));
                /* Eval embedded scripts */
                let embeddedScripts = allScriptTags.filter(s => !s.src).map(s => s.innerHTML);
                embeddedScripts.forEach(safeEval);
                /* Eval remote scripts */
                allScriptTags.filter(s => s.src).map(s => s.src).forEach(url => {
                    fetch(url).then(r => r.text()).then(safeEval);
                });

                /* Scroll to top */
                window.scrollTo(0, 0);

                this.loadContentByClass();

                let meta = window.meta || {
                    title : titles ? titles[0].innerText : 'Ulearn',
                    description: 'Интерактивные учебные онлайн-курсы по программированию',
                    keywords: '',
                    imageUrl: '',
                };
                this.setState(s => {
                    s.loading = false;
                    s.meta = meta;
                    return s;
                });
            }).catch(function(error) {
                console.error(error);
                /* Retry after timeout */
                setTimeout(() => self.fetchContentFromServer(url), 5000);
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
        let meta = Object.assign({}, this.state.meta);
        let links = this.state.links;
        return (
            <div>
                <Meta meta={meta} links={links}/>
                <Content body={this.state.body} />
            </div>
        )
    }

    loadContentByClass() {
        const className = 'load-content';
        let elements = Array.from(document.body.getElementsByClassName(className));
        elements.forEach(e => {
            let url = e.dataset.url;
            fetch(url, {credentials: 'include'}).then(r => r.text()).then(data => e.innerHTML = data);
        });
    }

    static contextTypes = {
        router: PropTypes.shape({
            history: PropTypes.shape({
                push: PropTypes.func.isRequired,
                replace: PropTypes.func.isRequired,
                createHref: PropTypes.func.isRequired
            }).isRequired
        }).isRequired
    };
}

class Content extends PureComponent {
    render() {
        return (<div dangerouslySetInnerHTML={{__html: this.props.body}}/>)
    }
}

class Meta extends Component {
    render() {
        let meta = this.props.meta;
        let links = this.props.links;
        let renderedLinks = [];
        for (let i = 0; i < links.length; i++) {
            let link = links[i];
            renderedLinks.push(<link rel={link.rel} type={link.type} href={link.href} key={i}/>);
        }
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
                {renderedLinks}
            </Helmet>
        )
    }
}

export default DownloadedHtmlContent;