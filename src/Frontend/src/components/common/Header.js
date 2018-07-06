import React, { Component } from 'react'
import * as PropTypes from 'prop-types'

import './Header.less'

class Header extends Component {

    render() {
        return (
            <div className="header">
                123123123123
            </div>
        )
    }

    static propTypes = {
        isAuthenticated: PropTypes.bool.isRequired
    }
}

export default Header;