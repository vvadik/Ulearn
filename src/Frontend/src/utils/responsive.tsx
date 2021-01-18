import React from 'react';
import Responsive from 'react-responsive';

/* See also variables.less for values */

export const Desktop = (props: unknown): React.ReactNode => <Responsive { ...props } minWidth={ 992 }/>;
export const Tablet = (props: unknown): React.ReactNode => <Responsive { ...props } minWidth={ 768 } maxWidth={ 991 }/>;
export const Mobile = (props: unknown): React.ReactNode => <Responsive { ...props } maxWidth={ 767 }/>;
export const NotMobile = (props: unknown): React.ReactNode => <Responsive { ...props } minWidth={ 768 }/>;

