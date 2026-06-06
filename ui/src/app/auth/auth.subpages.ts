import { loginRoute, registerRoute } from "../shared/constants/routes";

export class Subpages {
    title!: string;
    isActive!: boolean;
    route!: string;
    needUpper!: boolean;
}

export var authSubpages: Subpages[] = [
    { title: 'Login', isActive: false, route: loginRoute, needUpper: true },
    { title: 'Register', isActive: false, route: registerRoute, needUpper: true },
];
