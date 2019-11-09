import { Greeter } from './index';

test('My Greeter', () => {
    expect(Greeter('dude')).toBe('Hello dude');
});
