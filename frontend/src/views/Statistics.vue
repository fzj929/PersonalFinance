<template>
  <div class="stats-container">
    <van-nav-bar title="统计" />

    <div class="date-filter">
      <van-field
        v-model="startDate"
        is-link
        readonly
        label="开始日期"
        placeholder="选择开始日期"
        @click="showStartDatePicker = true"
      />
      <van-field
        v-model="endDate"
        is-link
        readonly
        label="结束日期"
        placeholder="选择结束日期"
        @click="showEndDatePicker = true"
      />
      <van-button type="primary" block @click="loadStatistics">查询</van-button>
    </div>

    <div v-if="statistics" class="stats-content">
      <div class="summary-cards">
        <van-grid :column-num="2" :border="false">
          <van-grid-item>
            <div class="stat-card income">
              <div class="stat-icon">💰</div>
              <div class="stat-number">¥{{ formatAmount(statistics.totalIncome) }}</div>
              <div class="stat-label">总收入</div>
            </div>
          </van-grid-item>
          <van-grid-item>
            <div class="stat-card expense">
              <div class="stat-icon">💸</div>
              <div class="stat-number">¥{{ formatAmount(statistics.totalExpense) }}</div>
              <div class="stat-label">总支出</div>
            </div>
          </van-grid-item>
          <van-grid-item>
            <div class="stat-card balance">
              <div class="stat-icon">💳</div>
              <div class="stat-number" :class="{ positive: statistics.balance >= 0, negative: statistics.balance < 0 }">
                ¥{{ formatAmount(statistics.balance) }}
              </div>
              <div class="stat-label">余额</div>
            </div>
          </van-grid-item>
          <van-grid-item>
            <div class="stat-card count">
              <div class="stat-icon">📊</div>
              <div class="stat-number">{{ statistics.transactionCount }}</div>
              <div class="stat-label">交易笔数</div>
            </div>
          </van-grid-item>
        </van-grid>
      </div>

      <div class="category-stats">
        <van-cell title="分类统计" />
        <van-cell
          v-for="item in statistics.categoryStatistics"
          :key="item.category"
          :title="item.category"
          :value="formatAmount(item.totalAmount)"
          :class="item.type"
        >
          <template #label>
            <span>{{ item.count }}笔</span>
          </template>
        </van-cell>
      </div>
    </div>

    <van-popup v-model:show="showStartDatePicker" round position="bottom">
      <van-date-picker
        v-model="currentStartDate"
        @confirm="onConfirmStartDate"
        @cancel="showStartDatePicker = false"
      />
    </van-popup>

    <van-popup v-model:show="showEndDatePicker" round position="bottom">
      <van-date-picker
        v-model="currentEndDate"
        @confirm="onConfirmEndDate"
        @cancel="showEndDatePicker = false"
      />
    </van-popup>

    <van-tabbar v-model="active" route>
      <van-tabbar-item name="home" icon="home-o" to="/">首页</van-tabbar-item>
      <van-tabbar-item name="stats" icon="chart-trending-o" to="/stats">统计</van-tabbar-item>
      <van-tabbar-item name="profile" icon="user-o" to="/profile">我的</van-tabbar-item>
    </van-tabbar>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { showNotify } from 'vant'
import dayjs from 'dayjs'
import api from '../services/api'

const active = ref('stats')
const startDate = ref(dayjs().startOf('month').format('YYYY-MM-DD'))
const endDate = ref(dayjs().format('YYYY-MM-DD'))
const showStartDatePicker = ref(false)
const showEndDatePicker = ref(false)
const statistics = ref(null)

const currentStartDate = ref([
  dayjs(startDate.value).format('YYYY'),
  dayjs(startDate.value).format('MM'),
  dayjs(startDate.value).format('DD')
])

const currentEndDate = ref([
  dayjs(endDate.value).format('YYYY'),
  dayjs(endDate.value).format('MM'),
  dayjs(endDate.value).format('DD')
])

const formatAmount = (amount) => {
  return Math.abs(amount).toFixed(2)
}

const onConfirmStartDate = (value) => {
  startDate.value = `${value.selectedValues[0]}-${value.selectedValues[1]}-${value.selectedValues[2]}`
  showStartDatePicker.value = false
}

const onConfirmEndDate = (value) => {
  endDate.value = `${value.selectedValues[0]}-${value.selectedValues[1]}-${value.selectedValues[2]}`
  showEndDatePicker.value = false
}

const loadStatistics = async () => {
  try {
    const response = await api.get('/transactions/statistics', {
      params: {
        startDate: startDate.value,
        endDate: endDate.value
      }
    })
    statistics.value = response.data
  } catch (error) {
    showNotify({ type: 'danger', message: '获取统计信息失败' })
  }
}

onMounted(loadStatistics)
</script>

<style scoped>
.stats-container {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 50px;
}

.date-filter {
  background: white;
  padding: 16px;
  margin-bottom: 16px;
}

.stats-content {
  padding: 0 16px;
}

.summary-cards {
  margin-bottom: 20px;
}

.stat-card {
  text-align: center;
  padding: 20px;
  border-radius: 8px;
  background: white;
}

.stat-card.income {
  background: linear-gradient(135deg, #87d068, #95de64);
  color: white;
}

.stat-card.expense {
  background: linear-gradient(135deg, #ff4d4f, #ff7875);
  color: white;
}

.stat-card.balance {
  background: linear-gradient(135deg, #1890ff, #69c0ff);
  color: white;
}

.stat-card.count {
  background: linear-gradient(135deg, #722ed1, #9254de);
  color: white;
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
  opacity: 0.9;
}

.category-stats {
  background: white;
  border-radius: 8px;
  overflow: hidden;
}

.income :deep(.van-cell__value) {
  color: #07c160;
}

.expense :deep(.van-cell__value) {
  color: #ee0a24;
}

.positive {
  color: #07c160;
}

.negative {
  color: #ee0a24;
}
</style>