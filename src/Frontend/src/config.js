const config = require("./settings.json");

/* Replace api.endpoint to be ready for production environment */
const hostname = window.location.hostname;
if (hostname !== 'localhost' && hostname !== '127.0.0.1') {
    /* Just add "api." to hostname, i.e. ulearn.me → api.ulearn.me */
    config.api.endpoint = window.location.protocol + '//api.' + hostname + '/';
}

export default config;