import intersection from 'lodash/fp/intersection';

export const Greeter = (name: string): string => `Hello ${name}`;

export const IntersectThis = (a: string[], b: string[]): string => intersection(a, b).join(',');
