# API 文档 - 个人记账系统

## 基础信息

- **Base URL**: `http://localhost:5000/api`
- **认证方式**: Bearer Token (JWT)
- **数据格式**: JSON

## 认证接口

### 用户注册

**POST** `/auth/register`

请求体:
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

响应:
```json
{
  "token": "jwt_token",
  "userId": 1,
  "username": "string"
}
```

### 用户登录

**POST** `/auth/login`

请求体:
```json
{
  "username": "string",
  "password": "string"
}
```

响应:
```json
{
  "token": "jwt_token"
}
```

## 交易接口

### 获取交易列表

**GET** `/transactions`

查询参数:
- `startDate`: 开始日期 (可选)
- `endDate`: 结束日期 (可选)
- `type`: 类型 (income/expense) (可选)

响应:
```json
[
  {
    "id": 1,
    "amount": 100.50,
    "type": "income",
    "category": "工资",
    "description": "月度工资",
    "transactionDate": "2024-01-20T00:00:00",
    "createdAt": "2024-01-20T10:30:00",
    "userId": 1
  }
]
```

### 创建交易

**POST** `/transactions`

请求体:
```json
{
  "amount": 100.50,
  "type": "income",
  "category": "工资",
  "description": "月度工资",
  "transactionDate": "2024-01-20T00:00:00"
}
```

### 获取交易详情

**GET** `/transactions/{id}`

### 更新交易

**PUT** `/transactions/{id}`

### 删除交易

**DELETE** `/transactions/{id}`

## 统计接口

### 获取统计数据

**GET** `/transactions/statistics`

查询参数:
- `startDate`: 开始日期 (必需)
- `endDate`: 结束日期 (必需)

响应:
```json
{
  "totalIncome": 1500.00,
  "totalExpense": 800.50,
  "balance": 699.50,
  "categoryStatistics": [
    {
      "category": "餐饮",
      "totalAmount": 300.00,
      "type": "expense",
      "count": 15
    }
  ],
  "transactionCount": 25
}
```

## 错误码

| 状态码 | 说明 |
|--------|------|
| 200 | 成功 |
| 201 | 创建成功 |
| 400 | 请求参数错误 |
| 401 | 未授权 |
| 404 | 资源不存在 |
| 500 | 服务器内部错误 |

## 数据模型

### User 模型
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Transaction 模型
```csharp
public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } // "income" or "expense"
    public string Category { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
}
```

## 使用示例

### JavaScript 示例
```javascript
// 登录获取token
const login = async () => {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username: 'test', password: '123456' })
  });

  const data = await response.json();
  localStorage.setItem('token', data.token);
};

// 获取交易列表
const getTransactions = async () => {
  const token = localStorage.getItem('token');

  const response = await fetch('/api/transactions', {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    }
  });

  return await response.json();
};
```

---

*最后更新: 2024-01-20*
*文档版本: v1.0*