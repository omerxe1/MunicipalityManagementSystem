// Kocaali Belediyesi - Panel JavaScript Kodları

// Duyurular verisi (örnek JSON)
const announcementsData = [
  {
    id: 1,
    title: "Kadınlara Özel Spor Salonu",
    date: "20 Aralık 2024",
    icon: "fas fa-dumbbell"
  },
  {
    id: 2,
    title: "Ulaşım Bizden Başarı Sizden",
    date: "18 Aralık 2024",
    icon: "fas fa-bus"
  },
  {
    id: 3,
    title: "Kale Mh. 153 ve 154 Sayılı Adalar İmar P...",
    date: "15 Aralık 2024",
    icon: "fas fa-home"
  },
  {
    id: 4,
    title: "Yangınlarla Mücadeleye Dev Destek",
    date: "12 Aralık 2024",
    icon: "fas fa-fire"
  },
  {
    id: 5,
    title: "Okul Destek Yardım Başvurusu",
    date: "10 Aralık 2024",
    icon: "fas fa-graduation-cap"
  },
  {
    id: 6,
    title: "Açık Hava Çocuk Etkinlikleri",
    date: "8 Aralık 2024",
    icon: "fas fa-child"
  },
  {
    id: 7,
    title: "4.Geleneksel Sokak Basketbolu",
    date: "5 Aralık 2024",
    icon: "fas fa-basketball-ball"
  },
  {
    id: 8,
    title: "Robotik Kodlama Kursu (Temmuz 2025)",
    date: "3 Aralık 2024",
    icon: "fas fa-code"
  },
  {
    id: 9,
    title: "E-İmar Sistemimiz En Güncel Haliyle Hizm...",
    date: "1 Aralık 2024",
    icon: "fas fa-building"
  },
  {
    id: 10,
    title: "30.Geleneksel Yusufeliler Yayla Festival...",
    date: "28 Kasım 2024",
    icon: "fas fa-mountain"
  }
];

// Haberler verisi (örnek JSON)
const newsData = [
  {
    id: 1,
    title: "Kocaali Belediyesi'nde Yeni Projeler Başladı",
    description: "Belediyemiz tarafından hayata geçirilen yeni projeler vatandaşlarımızın hizmetine sunuldu.",
    image: "https://via.placeholder.com/400x250/4A90E2/FFFFFF?text=Haber+1",
    date: "20 Aralık 2024"
  },
  {
    id: 2,
    title: "Kocaali'de Kültür ve Sanat Etkinlikleri",
    description: "Şehrimizde düzenlenen kültür ve sanat etkinlikleri büyük ilgi görüyor.",
    image: "https://via.placeholder.com/400x250/7ED321/FFFFFF?text=Haber+2",
    date: "18 Aralık 2024"
  },
  {
    id: 3,
    title: "Belediye Hizmetleri Dijitalleşiyor",
    description: "Vatandaşlarımız artık belediye hizmetlerine online olarak ulaşabilecek.",
    image: "https://via.placeholder.com/400x250/F5A623/FFFFFF?text=Haber+3",
    date: "15 Aralık 2024"
  }
];

// Sayfa yüklendiğinde çalışacak fonksiyonlar
document.addEventListener('DOMContentLoaded', function() {
  // Duyurular artık Razor ile veritabanından render ediliyor, JavaScript ile yüklenmiyor
  // loadAnnouncements(); // Devre dışı bırakıldı - veritabanından geliyor
  
  // Haberleri yükle
  loadNews();
  
  // Facebook iframe yüklenme kontrolü
  checkFacebookIframe();
});

// Duyuruları DOM'a ekle
function loadAnnouncements() {
  const announcementsList = document.getElementById('announcementsList');
  
  if (!announcementsList) return;
  
  // Mevcut içeriği temizle
  announcementsList.innerHTML = '';
  
  // Duyuruları ekle
  announcementsData.forEach(announcement => {
    const announcementElement = createAnnouncementElement(announcement);
    announcementsList.appendChild(announcementElement);
  });
}

// Duyuru elementi oluştur
function createAnnouncementElement(announcement) {
  const div = document.createElement('div');
  div.className = 'announcement-item';
  div.innerHTML = `
    <div class="announcement-content">
      <div class="announcement-title">${announcement.title}</div>
      <div class="announcement-date">${announcement.date}</div>
    </div>
  `;
  
  // Tıklama olayı ekle
  div.addEventListener('click', function() {
    console.log('Duyuru tıklandı:', announcement.title);
    // Burada duyuru detay sayfasına yönlendirme yapılabilir
  });
  
  return div;
}

// Haberleri yükle (şu anda HTML'de sabit, gelecekte dinamik olabilir)
function loadNews() {
  // Haberler şu anda HTML'de Bootstrap Carousel olarak tanımlı
  // Gelecekte burada API'den veri çekip dinamik olarak oluşturulabilir
  console.log('Haberler yüklendi:', newsData.length, 'adet');
}

// Facebook iframe yüklenme kontrolü
function checkFacebookIframe() {
  const iframe = document.querySelector('.facebook-iframe-container iframe');
  const fallback = document.querySelector('.facebook-fallback');
  
  if (!iframe || !fallback) return;
  
  // Iframe yüklenme olayı
  iframe.addEventListener('load', function() {
    console.log('Facebook iframe yüklendi');
  });
  
  // Iframe yüklenme hatası
  iframe.addEventListener('error', function() {
    console.log('Facebook iframe yüklenemedi, fallback gösteriliyor');
    document.querySelector('.facebook-iframe-container').style.display = 'none';
    fallback.style.display = 'block';
  });
  
  // 5 saniye sonra hala yüklenmemişse fallback göster
  setTimeout(() => {
    if (iframe.offsetHeight === 0) {
      document.querySelector('.facebook-iframe-container').style.display = 'none';
      fallback.style.display = 'block';
    }
  }, 5000);
}

// Carousel otomatik oynatma kontrolü
function initCarouselControls() {
  const carousel = document.getElementById('newsCarousel');
  
  if (!carousel) return;
  
  // Carousel hover'da duraklat
  carousel.addEventListener('mouseenter', function() {
    const bsCarousel = bootstrap.Carousel.getInstance(carousel);
    if (bsCarousel) {
      bsCarousel.pause();
    }
  });
  
  // Carousel hover'dan çıkınca devam et
  carousel.addEventListener('mouseleave', function() {
    const bsCarousel = bootstrap.Carousel.getInstance(carousel);
    if (bsCarousel) {
      bsCarousel.cycle();
    }
  });
}

// Panel animasyonları
function initPanelAnimations() {
  const panels = document.querySelectorAll('.panel-card');
  
  // Intersection Observer ile scroll animasyonları
  const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.style.opacity = '1';
        entry.target.style.transform = 'translateY(0)';
      }
    });
  }, {
    threshold: 0.1
  });
  
  panels.forEach(panel => {
    panel.style.opacity = '0';
    panel.style.transform = 'translateY(30px)';
    panel.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
    observer.observe(panel);
  });
}

// Responsive kontrol
function handleResponsiveChanges() {
  const facebookIframe = document.querySelector('.facebook-iframe-container');
  const facebookFallback = document.querySelector('.facebook-fallback');
  
  function checkScreenSize() {
    if (window.innerWidth <= 768) {
      // Mobilde iframe'i gizle, fallback'i göster
      if (facebookIframe) facebookIframe.style.display = 'none';
      if (facebookFallback) facebookFallback.style.display = 'block';
    } else {
      // Desktop'ta iframe'i göster, fallback'i gizle
      if (facebookIframe) facebookIframe.style.display = 'block';
      if (facebookFallback) facebookFallback.style.display = 'none';
    }
  }
  
  // İlk kontrol
  checkScreenSize();
  
  // Resize olayı
  window.addEventListener('resize', checkScreenSize);
}

// Tüm fonksiyonları başlat
document.addEventListener('DOMContentLoaded', function() {
  initCarouselControls();
  initPanelAnimations();
  handleResponsiveChanges();
});

// Export fonksiyonları (test için)
window.PanelManager = {
  loadAnnouncements,
  loadNews,
  checkFacebookIframe,
  announcementsData,
  newsData
};

// ================= NAVBAR SCROLL EFFECT - DEBUG =================
(function() {
  var navbar = document.querySelector('.main-navbar');
  var topbar = document.querySelector('.topbar');
  var scrollThreshold = 50; // Kaç px sonra etkinleşsin

  function checkNavbarScroll() {
    console.log('scroll event — Y:', window.scrollY, '| navbar:', !!navbar, '| topbar:', !!topbar);
    if(window.scrollY > scrollThreshold) {
      navbar.classList.add('scrolled', 'scroll-compact');
      if(topbar) topbar.classList.add('hidden', 'scroll-hidden');
      document.body.classList.add('scrolled');
      console.log('scrolled: class added (scrolled/scroll-compact/body.scrolled)');
    } else {
      navbar.classList.remove('scrolled', 'scroll-compact');
      if(topbar) topbar.classList.remove('hidden', 'scroll-hidden');
      document.body.classList.remove('scrolled');
      console.log('scrolled: class removed (scrolled/scroll-compact/body.scrolled)');
    }
  }

  window.addEventListener('scroll', checkNavbarScroll, {passive: true});
  checkNavbarScroll();
})();

// Çalışmalar Section - Project Navigation
// Statik veri (fallback - veritabanından veri gelmezse kullanılacak)
const defaultProjectsData = [
  {
    id: 1,
    title: "Meydan Kocaali",
    image: "https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=400&h=250&fit=crop&crop=center",
    description: "Şehir merkezinde modern meydan projesi"
  },
  {
    id: 2,
    title: "Kültür Merkezi",
    image: "https://images.unsplash.com/photo-1511818966892-d7d671e672a2?w=400&h=250&fit=crop&crop=center",
    description: "Çok amaçlı kültür ve sanat merkezi"
  },
  {
    id: 3,
    title: "Spor Kompleksi",
    image: "https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=400&h=250&fit=crop&crop=center",
    description: "Modern spor tesisleri ve kompleks"
  },
  {
    id: 4,
    title: "Park ve Bahçeler",
    image: "https://images.unsplash.com/photo-1441974231531-c6227db76b6e?w=400&h=250&fit=crop&crop=center",
    description: "Yeşil alanlar ve rekreasyon projeleri"
  },
  {
    id: 5,
    title: "Ulaşım Projeleri",
    image: "https://images.unsplash.com/photo-1544620347-c4fd4a3d5957?w=400&h=250&fit=crop&crop=center",
    description: "Modern ulaşım altyapısı geliştirmeleri"
  }
];

// Veritabanından gelen verileri kullan, yoksa statik veriyi kullan
function getProjectsData() {
  // window.calismalarData partial view tarafından set edilir
  if (window.calismalarData && Array.isArray(window.calismalarData) && window.calismalarData.length > 0) {
    // Veritabanından gelen verileri projectsData formatına dönüştür
    return window.calismalarData.map(calisma => ({
      id: calisma.id,
      title: calisma.title,
      image: calisma.imageUrl || "https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=600&h=400&fit=crop&crop=center",
      description: calisma.description
    }));
  }
  return defaultProjectsData;
}

let currentProjectIndex = 0;

// Project navigation functions
function nextProject() {
  const projectsData = getProjectsData();
  currentProjectIndex = (currentProjectIndex + 1) % projectsData.length;
  updateProjectDisplay();
}

function previousProject() {
  const projectsData = getProjectsData();
  currentProjectIndex = (currentProjectIndex - 1 + projectsData.length) % projectsData.length;
  updateProjectDisplay();
}

function updateProjectDisplay() {
  console.log('updateProjectDisplay çağrıldı, currentProjectIndex:', currentProjectIndex);
  
  const projectImage = document.getElementById('projectImage');
  const projectTitle = document.getElementById('projectTitle');
  
  console.log('projectImage bulundu:', projectImage);
  console.log('projectTitle bulundu:', projectTitle);
  
  if (projectImage && projectTitle) {
    const projectsData = getProjectsData();
    const currentProject = projectsData[currentProjectIndex];
    console.log('Güncellenen proje:', currentProject);
    
    // Smooth transition effect
    projectImage.style.opacity = '0';
    projectTitle.style.opacity = '0';
    
    setTimeout(() => {
      projectImage.src = currentProject.image;
      projectImage.alt = currentProject.title;
      projectTitle.textContent = currentProject.title;
      
      projectImage.style.opacity = '1';
      projectTitle.style.opacity = '1';
    }, 150);
  } else {
    console.log('projectImage veya projectTitle bulunamadı!');
  }
}

// Initialize project navigation
function initProjectNavigation() {
  console.log('initProjectNavigation başlatıldı');
  
  // Add keyboard navigation
  document.addEventListener('keydown', function(e) {
    if (e.key === 'ArrowLeft') {
      previousProject();
    } else if (e.key === 'ArrowRight') {
      nextProject();
    }
  });
  
  // Add touch/swipe support for mobile
  let touchStartX = 0;
  let touchEndX = 0;
  
  const projectCard = document.querySelector('.project-card');
  if (projectCard) {
    projectCard.addEventListener('touchstart', function(e) {
      touchStartX = e.changedTouches[0].screenX;
    });
    
    projectCard.addEventListener('touchend', function(e) {
      touchEndX = e.changedTouches[0].screenX;
      handleSwipe();
    });
  }
  
  function handleSwipe() {
    const swipeThreshold = 50;
    const diff = touchStartX - touchEndX;
    
    if (Math.abs(diff) > swipeThreshold) {
      if (diff > 0) {
        // Swipe left - next project
        nextProject();
      } else {
        // Swipe right - previous project
        previousProject();
      }
    }
  }
  
  // Initialize with first project
  updateProjectDisplay();
}

// Auto-advance projects (optional)
function startAutoAdvance() {
  setInterval(() => {
    nextProject();
  }, 8000); // Change project every 8 seconds
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
  console.log('DOMContentLoaded - Çalışmalar section kontrolü başladı');
  
  // Check if we're on the Haberler page
  const calismalarSection = document.querySelector('.calismalar-section');
  console.log('Çalışmalar section bulundu:', calismalarSection);
  
  if (calismalarSection) {
    console.log('Çalışmalar section bulundu, navigation başlatılıyor');
    initProjectNavigation();
    // Uncomment the line below to enable auto-advance
    // startAutoAdvance();
  } else {
    console.log('Çalışmalar section bulunamadı!');
  }
});

// Export project functions for external use
window.ProjectNavigation = {
  nextProject,
  previousProject,
  updateProjectDisplay,
  getProjectsData,
  currentProjectIndex
};
