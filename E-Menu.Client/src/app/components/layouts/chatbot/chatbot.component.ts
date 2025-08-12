import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpService } from '../../../services/http.service';
import { ResultModel } from '../../../models/result.model';
import { ConversationModel } from '../../../models/conversation.model';

interface ChatMessage {
  sender: 'user' | 'bot';
  text: string;
  timestamp: Date;
}

@Component({
  selector: 'app-chatbot',
  templateUrl: './chatbot.component.html',
  standalone: true,
  styleUrls: ['./chatbot.component.css'],
  imports: [CommonModule, FormsModule],
})
export class ChatBotComponent implements OnInit, OnDestroy {
  public messages: ChatMessage[] = [];
  public userMessage = '';
  public isConnected = false;
  public isTyping = false;
  public isChatOpen = true; // Sohbet penceresinin açık olup olmadığını kontrol etmek için
  private socket: WebSocket | null = null;
  private conversationId: string = '';
  private token: string = '';
  private userId: string = `user-${new Date().getTime()}`;
  private tokenRefreshTimer: any = null;

  constructor(private http: HttpService) {}

  ngOnInit(): void {
    this.startConversation();
  }

  ngOnDestroy(): void {
    this.closeConnection();
    if (this.tokenRefreshTimer) {
      clearTimeout(this.tokenRefreshTimer);
    }
  }

  private startConversation(): void {
    this.http.post<ConversationModel>(
      'conversations/start',
      {},
      (response) => {
        const data = response.value!;
        console.log('Konuşma başlatıldı:', data);
        this.conversationId = data.conversationId;
        this.token = data.token;

        // WebSocket bağlantısı kur
        this.connectWebSocket(data.streamUrl);

        // Hoş geldin mesajı ekle
        this.addMessage('bot', 'Merhaba! Size nasıl yardımcı olabilirim?');

        // Token süresi dolmadan önce yenileme planla
        if (data.expiresIn) {
          const refreshTime = (data.expiresIn - 5 * 60) * 1000; // milisaniye cinsinden
          console.log(
            `Token ${data.expiresIn} saniye sonra dolacak, ${refreshTime}ms sonra yenilenecek`
          );

          this.tokenRefreshTimer = setTimeout(() => {
            console.log('Token yenileniyor...');
            this.startConversation();
          }, refreshTime);
        }
      },
      (err) => {
        console.error('Konuşma başlatılırken hata oluştu:', err);
        this.addMessage(
          'bot',
          'Üzgünüm, bir hata oluştu. Lütfen daha sonra tekrar deneyin.'
        );
      }
    );
  }

  private connectWebSocket(url: string): void {
    // Önceki bağlantıyı kapat
    this.closeConnection();

    // Yeni WebSocket bağlantısı oluştur
    this.socket = new WebSocket(url);

    // Bağlantı açıldığında
    this.socket.onopen = () => {
      console.log('WebSocket bağlantısı kuruldu');
      this.isConnected = true;
    };

    // Mesaj alındığında
    this.socket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log('Alınan veri:', data);

      // Mesajları işle
      if (data.activities && data.activities.length > 0) {
        data.activities.forEach((activity: any) => {
          if (activity.type === 'message' && activity.from.id !== this.userId) {
            // Bot mesajını ekle
            this.isTyping = false;
            this.addMessage('bot', activity.text || 'İçerik görüntülenemiyor');
          } else if (
            activity.type === 'typing' &&
            activity.from.id !== this.userId
          ) {
            // Bot yazıyor göstergesini göster
            this.isTyping = true;
          }
        });
      }
    };

    // Hata oluştuğunda
    this.socket.onerror = (error) => {
      console.error('WebSocket hatası:', error);
      this.isConnected = false;
    };

    // Bağlantı kapandığında
    this.socket.onclose = (event) => {
      console.log('WebSocket bağlantısı kapandı:', event.code, event.reason);
      this.isConnected = false;

      // Bağlantı beklenmedik şekilde kapandıysa yeniden bağlanmayı dene
      if (event.code !== 1000) {
        console.log('Yeniden bağlanmaya çalışılıyor...');
        setTimeout(() => this.startConversation(), 3000);
      }
    };
  }

  private closeConnection(): void {
    if (this.socket) {
      if (this.socket.readyState === WebSocket.OPEN) {
        this.socket.close(1000, 'Kullanıcı bağlantıyı kapattı');
      }
      this.socket = null;
    }
    this.isConnected = false;
  }

  public sendMessage(): void {
    if (!this.userMessage.trim() || !this.isConnected) return;

    // Kullanıcı mesajını UI'a ekle
    this.addMessage('user', this.userMessage);

    // DirectLine'a mesaj gönder
    const activity = {
      conversationId: this.conversationId,
      userId: this.userId,
      message: this.userMessage,
    };

    // HTTP ile mesaj gönder
    this.http.post<boolean>(`conversations/sendMessage`, activity, (res) => {
      if (res.value) {
        console.log('Mesaj başarıyla gönderildi');
      } else {
        console.error('Mesaj gönderilirken hata oluştu');
      }
    });

    // Mesaj kutusunu temizle
    this.userMessage = '';

    // Bot yazıyor göstergesini göster
    this.isTyping = true;
  }

  private addMessage(sender: 'user' | 'bot', text: string): void {
    this.messages.push({
      sender,
      text,
      timestamp: new Date(),
    });

    // DOM güncellemesi için bir tick bekle, sonra kaydır
    setTimeout(() => {
      const chatContainer = document.querySelector('.chat-messages');
      if (chatContainer) {
        chatContainer.scrollTop = chatContainer.scrollHeight;
      }
    }, 0);
  }
}
