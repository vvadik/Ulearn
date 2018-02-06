import React, { Component } from 'react'
import { Navbar } from 'react-bootstrap'

class NavbarIcons extends Component {
    render() {
        return (
            <button type="button" className="navbar-toggle hide-side-bar-button" data-toggle="collapse" data-target={this.props.target}>
                <span className="icon-bar"/>
                <span className="icon-bar"/>
                <span className="icon-bar"/>
            </button>
        )
    }
}
 
class Header extends Component {
    render() {
        return (
            <Navbar fixedTop inverse>
                <NavbarIcons target=".side-bar"/>
                <div className="container">
                    <Navbar.Header>
                        <NavbarIcons target=".greeting-collapse-class"/>
                        <Navbar.Brand>
                            <a href="/Home">Ulearn</a>
                        </Navbar.Brand>
                        <ul className="notifications__mobile-nav nav navbar-nav pull-right visible-xs">
                            @if (User.Identity.IsAuthenticated)
                            {
                                <li className="dropdown">
                                    <a href="Feed/NotificationsTopbarPartial?isMobile=true" />
                                </li>
                            }
                        </ul>
                    </Navbar.Header>
                    <ul className="notifications__mobile-dropdown notifications__dropdown navbar-collapse collapse visible-xs">

                    </ul>
                    <div className="greeting-collapse-class navbar-collapse collapse">
                        <ul className="top-navigation nav navbar-nav">
                        </ul>
                    </div>
                </div>
            </Navbar>            
        )
    }
}

export default Header;