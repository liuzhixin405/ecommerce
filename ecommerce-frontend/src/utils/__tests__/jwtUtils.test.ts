import { parseJwtToken, isTokenExpired, getUserFromToken, isValidToken } from '../jwtUtils';

// 测试用的JWT token (这是一个示例token，实际使用时需要真实的token)
const testToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';

describe('JWT Utils', () => {
  test('parseJwtToken should parse valid token', () => {
    const payload = parseJwtToken(testToken);
    expect(payload).toBeTruthy();
    expect(payload?.sub).toBe('1234567890');
  });

  test('parseJwtToken should return null for invalid token', () => {
    const payload = parseJwtToken('invalid.token.here');
    expect(payload).toBeNull();
  });

  test('isTokenExpired should check expiration correctly', () => {
    // 创建一个过期的token
    const expiredPayload = {
      sub: '123',
      exp: Math.floor(Date.now() / 1000) - 3600, // 1小时前过期
      iat: Math.floor(Date.now() / 1000) - 7200
    };
    
    const expiredToken = 'header.' + btoa(JSON.stringify(expiredPayload)) + '.signature';
    expect(isTokenExpired(expiredToken)).toBe(true);
  });

  test('getUserFromToken should extract user info', () => {
    const userInfo = getUserFromToken(testToken);
    expect(userInfo).toBeTruthy();
    expect(userInfo?.id).toBe('1234567890');
  });

  test('isValidToken should validate token correctly', () => {
    expect(isValidToken('')).toBe(false);
    expect(isValidToken('invalid')).toBe(false);
  });
});
