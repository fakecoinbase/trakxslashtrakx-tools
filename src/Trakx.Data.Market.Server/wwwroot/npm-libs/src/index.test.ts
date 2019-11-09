import { Greeter, IntersectThis } from './index';

test('My Greeter', () => {
    expect(Greeter('dude')).toBe('Hello dude');
});

test('Intersector', () => {
    const a = ['hello', 'not', 'you'];
    const b = ['hello', 'you', 'toi'];
    const intersection = IntersectThis(a, b);
    expect(intersection).toBe('hello,you');
});
