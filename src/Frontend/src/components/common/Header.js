import React, { Component } from 'react'
import * as PropTypes from 'prop-types'
import {Loader, MenuItem, MenuSeparator, Tooltip} from "@skbkontur/react-ui/components/all";
import Icon from "@skbkontur/react-ui/Icon"
import MenuHeader from "@skbkontur/react-ui/MenuHeader"
import DropdownMenu from "@skbkontur/react-ui/DropdownMenu"
import DropdownContainer from "@skbkontur/react-ui/components/DropdownContainer/DropdownContainer"
import {Link, withRouter} from "react-router-dom";
import { connect } from "react-redux";
import { findDOMNode } from "react-dom"

import './Header.less'
import {getQueryStringParameter} from "../../utils";

import api from "../../api"


let accountPropTypes = PropTypes.shape({
    isAuthenticated: PropTypes.bool.isRequired,
    login: PropTypes.string,
    firstName: PropTypes.string,
    lastName: PropTypes.string,
    isSystemAdministrator: PropTypes.bool.isRequired,
    roleByCourse: PropTypes.shape().isRequired,
    accessesByCourse: PropTypes.shape().isRequired,
    accountProblems: PropTypes.arrayOf(PropTypes.shape)
}).isRequired;

class Header extends Component {
    render() {
        let roleByCourse = this.props.account.roleByCourse;
        let accessesByCourse = this.props.account.accessesByCourse;

        let controllableCourseIds = Object.keys(roleByCourse).filter(courseId => roleByCourse[courseId] !== 'tester');
        if (this.props.account.isSystemAdministrator)
            controllableCourseIds = Object.keys(this.props.courses.courseById);
        let isCourseMenuVisible = (
            this.props.courses &&
            this.props.courses.currentCourseId &&
            controllableCourseIds.indexOf(this.props.courses.currentCourseId) !== -1
        );

        let courseRole = "";
        let courseAccesses = [];
        if (isCourseMenuVisible) {
            let courseId = this.props.courses.currentCourseId;
            if (this.props.account.isSystemAdministrator)
                courseRole = 'CourseAdmin';
            else
                courseRole = roleByCourse[courseId];
            courseAccesses = accessesByCourse[courseId] || [];
        }

        return (
            <div className="header">
                <Logo>
                    <span className="visible-only-phone"><Icon name="Home"/></span>
                    <span className="visible-at-least-tablet">Ulearn.me</span>
                </Logo>

                { this.props.account.isSystemAdministrator && <SysAdminMenu controllableCourseIds={controllableCourseIds}/> }
                { ! this.props.account.isSystemAdministrator && controllableCourseIds.length > 0 && <MyCoursesMenu controllableCourseIds={controllableCourseIds}/>}
                { isCourseMenuVisible && <CourseMenu courseId={ this.props.courses.currentCourseId } role={ courseRole } accesses={ courseAccesses }/> }
                <Menu account={this.props.account}/>
            </div>
        )
    }

    static mapStateToProps(state) {
        return {
            account: state.account,
            courses: state.courses,
        }
    }

    static propTypes = {
        account: accountPropTypes
    }
}

class Logo extends Component {
    render() {
        return (
            <div className="header__logo">
                <Link to="/">
                    { this.props.children }
                </Link>
            </div>
        )
    }
}

class AbstractMyCoursesMenu extends Component {
    VISIBLE_COURSES_COUNT = 10;

    _getCourseMenuItems(courseIds) {
        const courseById = this.props.courses.courseById;
        courseIds.sort((a, b) => courseById[a].title.localeCompare(courseById[b].title));
        let visibleCourseIds = courseIds.slice(0, this.VISIBLE_COURSES_COUNT);
        let items = visibleCourseIds.filter(courseId => courseById.hasOwnProperty(courseId)).map(courseId =>
            <MenuItem href={"/Course/" + courseId } key={courseId}>{ courseById[courseId].title }</MenuItem>
        );
        if (Object.keys(courseById).length > visibleCourseIds.length)
            items.push(<MenuItem href="/Admin/CourseList" key="-course-list"><strong>Все курсы</strong></MenuItem>);
        return items;
    }

    static propTypes = {
        controllableCourseIds: PropTypes.arrayOf(PropTypes.string).isRequired,
    };

    static mapStateToProps(state) {
        return {
            courses: state.courses
        }
    }
}

class SysAdminMenu extends AbstractMyCoursesMenu {
    render() {
        return (
            <div className="header__sysadmin-menu">
                <DropdownMenu
                    caption={
                        <div>
                            <span className="visible-only-phone"><span className="icon"><Icon name="DocumentGroup"/></span></span>
                            <span className="caption visible-at-least-tablet">Администрирование <span className="caret"/></span>
                        </div>
                    }
                >
                    <MenuItem href="/Account/List?role=sysAdmin">Пользователи</MenuItem>
                    <MenuItem href="/Analytics/SystemStatistics">Статистика</MenuItem>
                    <MenuItem href="/Sandbox">Песочница C#</MenuItem>
                    <MenuItem href="/Admin/StyleValidations">Стилевые ошибки C#</MenuItem>
                    <MenuSeparator />
                    <MenuHeader>Курсы</MenuHeader>
                    { this._getCourseMenuItems(this.props.controllableCourseIds) }
                </DropdownMenu>
            </div>
        )
    }
}

SysAdminMenu = connect(SysAdminMenu.mapStateToProps)(SysAdminMenu);

class MyCoursesMenu extends AbstractMyCoursesMenu {
    render() {
        return (
            <div className="header__my-courses-menu">
                <DropdownMenu
                    caption={
                        <div>
                            <span className="visible-only-phone"><span className="icon"><Icon name="DocumentGroup"/></span></span>
                            <span className="caption visible-at-least-tablet">Мои курсы <span className="caret"/></span>
                        </div>
                    }
                >
                    { this._getCourseMenuItems(this.props.controllableCourseIds) }
                </DropdownMenu>
            </div>
        )
    }
}

MyCoursesMenu = connect(MyCoursesMenu.mapStateToProps)(MyCoursesMenu);

class CourseMenu extends Component {
    render() {
        let { courseId, role, accesses } = this.props;
        let course = this.props.courses.courseById[courseId];
        if (typeof course === 'undefined')
            return null;

        let usersMenuItem = [];
        if (role === 'CourseAdmin' || accesses.indexOf('addAndRemoveInstructors') !== -1)
            usersMenuItem = [<MenuItem href={"/Admin/Users?courseId=" + courseId} key="Users">Пользователи</MenuItem>];
        let courseAdminMenuItems = [];
        if (role === 'CourseAdmin')
            courseAdminMenuItems = [
                <MenuItem href={"/Admin/Packages?courseId=" + courseId} key="Packages">Загрузить пакет</MenuItem>,
                <MenuItem href={"/Admin/Units?courseId=" + courseId} key="Units">Модули</MenuItem>,
                <MenuItem href={"/Grader/Clients?courseId=" + courseId} key="GraderClients">Клиенты грейдера</MenuItem>
            ];

        return (
            <div className="header__course-menu">
                <DropdownMenu
                    caption={<div>
                        <span className="visible-only-phone"><span className="icon"><Icon name="DocumentSolid"/></span></span>
                        <span className="caption visible-at-least-tablet" title={ course.title }><span className="courseName">{ course.title }</span> <span className="caret"/></span>
                    </div>}
                    menuWidth={300}
                >
                    <MenuItem href={"/Course/" + courseId}>Просмотр курса</MenuItem>
                    <MenuSeparator />
                    <MenuItem href={"/Admin/Groups?courseId=" + courseId}>Группы</MenuItem>
                    <MenuItem href={"/Analytics/CourseStatistics?courseId=" + courseId}>Ведомость курса</MenuItem>
                    <MenuItem href={"/Analytics/UnitStatistics?courseId=" + courseId}>Ведомость модуля</MenuItem>
                    <MenuItem href={"/Admin/Certificates?courseId=" + courseId}>Сертификаты</MenuItem>
                    { (usersMenuItem.length > 0 || courseAdminMenuItems.length > 0) ? <MenuSeparator/> : ''}
                    { usersMenuItem }
                    { courseAdminMenuItems }
                    <MenuSeparator />
                    <MenuItem href={"/Admin/Comments?courseId=" + courseId}>Комментарии</MenuItem>
                    <MenuItem href={"/Admin/ManualQuizCheckingQueue?courseId=" + courseId}>Проверка тестов</MenuItem>
                    <MenuItem href={"/Admin/ManualExerciseCheckingQueue?courseId=" + courseId}>Код-ревью</MenuItem>
                </DropdownMenu>
            </div>
        )
    }

    static propTypes = {
        courseId: PropTypes.string.isRequired,
        courses: PropTypes.shape(),
        role: PropTypes.string.isRequired,
        accesses: PropTypes.arrayOf(PropTypes.string).isRequired
    };

    static mapStateToProps(state) {
        return {
            courses: state.courses
        }
    }
}

CourseMenu = connect(CourseMenu.mapStateToProps)(CourseMenu);

class Menu extends Component {
    render() {
        let returnUrl = this.props.location.pathname + this.props.location.search;
        if (returnUrl.startsWith("/Login") || returnUrl.startsWith("/Account/Register")) {
            returnUrl = getQueryStringParameter("returnUrl");
        }

        if (this.props.account.isAuthenticated) {
            return (
                <div className="header__menu">
                    <NotificationsMenu/>
                    <ProfileLink account={this.props.account}/>
                    <Separator/>
                    <LogoutLink/>
                </div>
            )
        } else {
            return (
                <div className="header__menu">
                    <RegistrationLink returnUrl={returnUrl }/>
                    <Separator/>
                    <LoginLink returnUrl={returnUrl }/>
                </div>
            )
        }
    }

    static propTypes = {
        account: accountPropTypes,
        location: PropTypes.object,
    }
}

Menu = withRouter(Menu);

class NotificationsMenu extends Component {
    constructor(props) {
        super(props);
        this.onClick = this.onClick.bind(this);

        this.state = {
            isOpened: false,
            isLoading: false,
            notificationsHtml: "",
            counter: props.notifications.count
        }
    }

    componentWillReceiveProps(nextProps, nextContext) {
        this.setState({
            counter: nextProps.notifications.count
        });
    }

    static _loadNotifications() {
        return fetch("/Feed/NotificationsPartial", { credentials: "include" }).then(
            response => response.text()
        )
    }

    onClick() {
        if (this.state.isOpened) {
            this.setState({
                isOpened: false,
            });
        } else {
            this.setState({
                isOpened: true,
                isLoading: true,
            });
            NotificationsMenu._loadNotifications().then(
                notifications => {
                    this.props.resetNotificationsCount();
                    this.setState({
                        isLoading: false,
                        notificationsHtml: notifications
                    })
                });
        }
    }

    render() {
        return (
            <div className={this.state.isOpened ? "opened" : ""}>
                <NotificationsIcon counter={this.state.counter} onClick={this.onClick}/>
                {
                    this.state.isOpened &&
                    <DropdownContainer getParent={() => findDOMNode(this)} offsetY={0} align="right">
                        <div className="dropdown-container">
                            <Notifications isLoading={this.state.isLoading} notifications={this.state.notificationsHtml}/>
                        </div>
                    </DropdownContainer>
                }
            </div>
        )
    }

    static mapStateToProps(state) {
        return {
            notifications: state.notifications
        }
    }

    static mapDispatchToProps(dispatch) {
        return {
            resetNotificationsCount: () => dispatch({
                type: 'NOTIFICATIONS__COUNT_RESETED'
            })
        };
    }
}

NotificationsMenu = connect(NotificationsMenu.mapStateToProps, NotificationsMenu.mapDispatchToProps)(NotificationsMenu);

class NotificationsIcon extends Component {
    render() {
        return (
            <div className={"header__notifications-icon " + (this.props.counter === 0 ? "without-counter" : "")} onClick={ this.props.onClick }>
                <span className="icon">
                    <Icon name="NotificationBell"/>
                </span>
                {
                    this.props.counter > 0 &&
                    <span className="counter">
                        { this.props.counter > 99 ? "99+" : this.props.counter}
                    </span>
                }
            </div>
        )
    }

    static propTypes = {
        counter: PropTypes.number.isRequired,
        onClick: PropTypes.func
    };

    static defaultProps = {
        onClick: () => 1
    };
}

class Notifications extends Component {
    render() {
        const {isLoading, notifications} = this.props;
        if (isLoading)
            return <Loader type="normal" active/>;
        else
            return <div className="notifications__dropdown" dangerouslySetInnerHTML={{ __html: notifications }} />;
    }

    static propTypes = {
        isLoading: PropTypes.bool.isRequired,
        notifications: PropTypes.string.isRequired
    }
}

class ProfileLink extends Component {
    constructor(props) {
        super(props);
        this.openTooltip = this.openTooltip.bind(this);
        this.closeTooltip = this.closeTooltip.bind(this);
        this.state = {
            tooltipTrigger: 'opened',
        }
    }

    openTooltip() {
        this.setState({
            tooltipTrigger: 'opened'
        })
    }

    closeTooltip() {
        this.setState({
            tooltipTrigger: 'closed'
        })
    }

    render() {
        let icon = <Icon name="User"/>;
        let isProblem = this.props.account.accountProblems.length > 0;
        if (isProblem) {
            let firstProblem = this.props.account.accountProblems[0];
            icon = (
                <Tooltip trigger={ this.state.tooltipTrigger } pos="bottom center" render={() => (
                    <div style={{width: '250px'}}>
                        {firstProblem.description}
                    </div>
                )} onCloseClick={ this.closeTooltip }>
                    <span onMouseOver={ this.openTooltip }>
                        <Icon name="Warning" color="#f77"/>
                    </span>
                </Tooltip>
            )
        }

        return (<div className="header__profile-link">
            <Link to="/Account/Manage">
                <span className="icon">
                    { icon }
                </span>
                <span className="username">
                    { this.props.account.visibleName || 'Профиль' }
                </span>
            </Link>
        </div>)
    }

    static propTypes = {
        account: accountPropTypes
    }
}

class Separator extends Component {
    render() {
        return <div className="header__separator"/>
    }
}

class LogoutLink extends Component {
    constructor(props) {
        super(props);
        this.onClick = this.onClick.bind(this);
    }

    onClick() {
        this.props.logout();
    }

    render() {
        return <div className="header__logout-link"><a href="#" onClick={ this.onClick }>Выйти</a></div>
    }

    static mapStateToProps(state) {
        return {};
    }

    static mapDispatchToProps(dispatch) {
        return {
            logout: () => dispatch(api.account.logout())
        };
    }
}

LogoutLink = connect(LogoutLink.mapStateToProps, LogoutLink.mapDispatchToProps)(LogoutLink);

class RegistrationLink extends Component {
    render() {
        return (
            <div className="header__registration-link">
                <Link to={ "/Account/Register?returnUrl=" + (this.props.returnUrl || "/")}>Зарегистрироваться</Link>
            </div>
        )
    }

    static propTypes = {
        returnUrl: PropTypes.string
    }
}

class LoginLink extends Component {
    render() {
        return (<div className="header__login-link">
            <Link to={ "/Login?returnUrl=" + (this.props.returnUrl || "/") }>Войти</Link>
        </div>)
    }

    static propTypes = {
        returnUrl: PropTypes.string
    }
}

export default connect(Header.mapStateToProps)(Header);