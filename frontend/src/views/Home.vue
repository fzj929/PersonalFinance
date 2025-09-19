<template>
  <div class="home-container">
    <van-nav-bar title="我的记账">
      <template #right>
        <van-icon name="search" size="18" @click="$router.push('/query')" />
      </template>
    </van-nav-bar>

    <div class="balance-card">
      <div class="balance-title">当前余额</div>
      <div class="balance-amount" :class="{ positive: totalBalance >= 0, negative: totalBalance < 0 }">
        ¥{{ formatAmount(totalBalance) }}
      </div>
      <div class="balance-stats">
        <div class="stat-item">
          <span class="stat-label">收入</span>
          <span class="stat-value income">¥{{ formatAmount(totalIncome) }}</span>
        </div>
        <div class="stat-item">
          <span class="stat-label">支出</span>
          <span class="stat-value expense">¥{{ formatAmount(totalExpense) }}</span>
        </div>
      </div>
    </div>

    <van-tabs v-model:active="activeTab" class="transactions-tabs">
      <van-tab title="最近交易">
        <div class="transactions-list">
          <van-empty v-if="transactions.length === 0" description="暂无交易记录" />

          <van-swipe-cell v-for="transaction in transactions" :key="transaction.id">
            <van-cell
              :title="transaction.description || transaction.category"
              :value="formatAmount(transaction.amount)"
              :class="transaction.type"
            >
              <template #label>
                <div class="transaction-info">
                  <span class="category">{{ transaction.category }}</span>
                  <span class="date">{{ formatDate(transaction.transactionDate) }}</span>
                </div>
              </template>
            </van-cell>

            <template #right>
              <van-button
                square
                type="danger"
                text="删除"
                @click="deleteTransaction(transaction.id)"
              />
            </template>
          </van-swipe-cell>
        </div>
      </van-tab>

      <van-tab title="快速统计">
        <div class="quick-stats">
          <van-grid :column-num="2" :border="false">
            <van-grid-item>
              <div class="stat-card">
                <div class="stat-icon income">💰</div>
                <div class="stat-number">¥{{ formatAmount(totalIncome) }}</div>
                <div class="stat-label">总收入</div>
              </div>
            </van-grid-item>
            <van-grid-item>
              <div class="stat-card">
                <div class="stat-icon expense">💸</div>
                <div class="stat-number">¥{{ formatAmount(totalExpense) }}</div>
                <div class="stat-label">总支出</div>
              </div>
            </van-grid-item>
          </van-grid>
        </div>
      </van-tab>
    </van-tabs>

    <van-floating-bubble
      v-model:offset="offset"
      axis="xy"
      magnetic="x"
      @click="$router.push('/add')"
    >
      <van-icon name="plus" size="24" />
    </van-floating-bubble>

    <van-tabbar v-model="active" route>
      <van-tabbar-item name="home" icon="home-o" to="/">首页</van-tabbar-item>
      <van-tabbar-item name="stats" icon="chart-trending-o" to="/stats">统计</van-tabbar-item>
      <van-tabbar-item name="profile" icon="user-o" to="/profile">我的</van-tabbar-item>
    </van-tabbar>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { showConfirmDialog, showNotify } from 'vant'
import dayjs from 'dayjs'
import api from '../services/api'

const active = ref('home')
const activeTab = ref(0)
const offset = ref({ x: 300, y: 600 })
const transactions = ref([])

const totalIncome = computed(() =>
  transactions.value.filter(t => t.type === 'income').reduce((sum, t) => sum + t.amount, 0)
)

const totalExpense = computed(() =>
  transactions.value.filter(t => t.type === 'expense').reduce((sum, t) => sum + t.amount, 0)
)

const totalBalance = computed(() => totalIncome.value - totalExpense.value)

const formatAmount = (amount) => {
  return Math.abs(amount).toFixed(2)
}

const formatDate = (date) => {
  return dayjs(date).format('MM-DD HH:mm')
}

const fetchTransactions = async () => {
  try {
    const limit = localStorage.getItem('transactionLimit') || '10'
    const response = await api.get(`/transactions?limit=${limit}`)
    transactions.value = response.data
  } catch (error) {
    showNotify({ type: 'danger', message: '获取交易记录失败' })
  }
}

const deleteTransaction = async (id) => {
  try {
    await showConfirmDialog({ message: '确定要删除这条记录吗？' })
    await api.delete(`/transactions/${id}`)
    await fetchTransactions()
    showNotify({ type: 'success', message: '删除成功' })
  } catch (error) {
    if (error !== 'cancel') {
      showNotify({ type: 'danger', message: '删除失败' })
    }
  }
}

onMounted(fetchTransactions)
</script>

<style scoped>
.home-container {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 50px;
}

.balance-card {
  background: white;
  margin: 16px;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.balance-title {
  font-size: 14px;
  color: #666;
  margin-bottom: 8px;
}

.balance-amount {
  font-size: 32px;
  font-weight: bold;
  margin-bottom: 16px;
}

.balance-amount.positive {
  color: #07c160;
}

.balance-amount.negative {
  color: #ee0a24;
}

.balance-stats {
  display: flex;
  justify-content: space-between;
}

.stat-item {
  text-align: center;
}

.stat-label {
  font-size: 12px;
  color: #999;
}

.stat-value {
  font-size: 16px;
  font-weight: bold;
}

.stat-value.income {
  color: #07c160;
}

.stat-value.expense {
  color: #ee0a24;
}

.transactions-list {
  margin: 16px;
}

.transaction-info {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
  color: #999;
}

.income :deep(.van-cell__value) {
  color: #07c160;
}

.expense :deep(.van-cell__value) {
  color: #ee0a24;
}

.quick-stats {
  padding: 16px;
}

.stat-card {
  text-align: center;
  padding: 20px;
}

.stat-icon {
  font-size: 24px;
  margin-bottom: 8px;
}

.stat-number {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 12px;
  color: #999;
}
</style>