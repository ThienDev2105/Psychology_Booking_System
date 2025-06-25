using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Serenity_Solution.Models;

namespace Serenity_Solution.Controllers
{
    public class PodcastController : Controller
    {
        private static List<PodcastViewModel> _podcasts = new List<PodcastViewModel>
        {
             new PodcastViewModel
            {
                Id = 1,
                Title = "Người ta có thương mình đâu",
                Description = "Có những tình cảm, mình biết ngay từ đầu là không thể.Vậy mà vẫn thương.Không đòi hỏi. Không hy vọng.Chỉ giữ trong lòng — như một điều đẹp nhất từng có. ",
                ImageUrl = "/image/Podcast/DeMemPhieuLuuKy.png",
                AudioUrl = "/Audio/NgTaKThuongMinh.mp3",
                Author = "Dế Mèn Du Ký",
                Duration = "20:54",
                Category = "Yêu thương & Kết nối",
                Rating = 4.8,
                RatingCount = 125
            },
            new PodcastViewModel
            {
                Id = 2,
                Title = "Bí quyết giúp con KHÔNG CÒN NHÚT NHÁT khi còn đi học",
                Description = "Xem phiên bản video:https://youtu.be/FY4qi0IUDtk?si=OFGhT-YXcrAOmpMY\r\n\r\nNhi Le Podcast cảm ơn bạn đã dành thời gian lắng nghe\r\n\r\n\r\nTheo dõi Fanpage tại https://www.facebook.com/AnneNhiLe và\r\n\r\n\r\nSubscribe kênh Youtube NHI LE https://www.youtube.com/@NHILE_SG để xem và cập nhật các kiến thức mới mỗi ngày",
                ImageUrl = "image/Podcast/NhiLe.png",
                AudioUrl = "/Audio/BiQuyetGiupCon.mp3",
                Author = "Nhi Lê",
                Duration = "11:38",
                Category = "Tâm lý & Cảm xúc",
                Rating = 4.5,
                RatingCount = 98
            },
            new PodcastViewModel
            {
                Id = 3,
                Title = "1%",
                Description = "Không phải bỏ cuộc, cũng không phải tạm dừng, nhưng có nghĩa là không ngừng cố gắng. #bettereveryday (ảnh cover từ concert Coldplay hồi tháng 2 mà quên không khoe trong podcast T_T)",
                ImageUrl = "/image/podcast/MayKeChuyen.png",
                AudioUrl = "/Audio/1%.mp3",
                Author = "Mây Kể Chuyện",
                Duration = "11:53",
                Category = "Trò chuyện chữa lành",
                Rating = 4.7,
                RatingCount = 142
            },
            new PodcastViewModel
            {
                Id = 4,
                Title = "Lời hồi đáp thanh xuân|Radio#28",
                Description = "Tháng 5 về, nắng bắt đầu rực rỡ hơn, ve bắt đầu râm ran gọi hè, những chùm bằng lăng tím biếc như gom cả bầu trời vào trong ký ức, cũng là lúc những các bạn học sinh cuối cấp bước đứng trước ngưỡng cửa quan trọng mang tên Đại học.",
                ImageUrl = "/image/podcast/ViSaoTheNhi.png",
                AudioUrl = "/Audio/LoiHoiDap.mp3",
                Author = "Vì Sao Thế Nhỉ",
                Duration = "14:50",
                Category = "Tâm lý & Cảm xúc",
                Rating = 4.9,
                RatingCount = 205
            },
            new PodcastViewModel
            {
                Id = 5,
                Title = "Ngày bão Kể Chuyện Bão Lòng",
                Description = "Khám phá những lời minh triết về cuộc sống từ các triết gia và nhà tâm lý học nổi tiếng, giúp bạn có cái nhìn sâu sắc hơn về cuộc sống.",
                ImageUrl = "/image/podcast/KeChoToiNghe.png",
                AudioUrl = "/Audio/CauCoThe.mp3",
                Author = "Vì sao thế nhỉ",
                Duration = "26:45",
                Category = "Phát triển bản thân",
                Rating = 4.6,
                RatingCount = 168
            },
            new PodcastViewModel
            {
                Id = 6,
                Title = "Buổi 15: Tùy Duyên - Nhảy múa cùng vũ trụ",
                Description = "Chúng con thương mời Quý đại chúng cùng lắng nghe bài giảng với chủ đề “Tùy Duyên - Nhảy múa cùng vũ trụ\" do Thầy Minh Niệm chia sẻ, thuộc Chuỗi Livestream “Tay Phật trong tay con” - Buổi 15.\r\nKính chúc Quý vị nhiều bình an và vững chãi.",
                ImageUrl = "/image/podcast/MinhNiem.png",
                AudioUrl = "/Audio/Buoi15.mp3",
                Author = "Minh Niệm",
                Duration = "58:12",
                Category = "Phát triển bản thân",
                Rating = 4.8,
                RatingCount = 183
            },
            new PodcastViewModel
            {
                Id = 7,

                Title = "Vì sao mình làm Podcast",
                Description = "Tình yêu không chỉ là cảm xúc, mà còn là sự đồng hành và hỗ trợ. Podcast này sẽ giúp bạn hiểu cách để thực sự đồng hành cùng người mình thương trong những lúc khó khăn.",
                ImageUrl = "/image/podcast/ViSaoLamPodcast.png",
                AudioUrl = "/Audio/ViSaoPod.mp3",
                Author = "Mây Kể Chuyện",
                Duration = "07:43",
                Category = "Yêu thương & Kết nối",
                Rating = 4.7,
                RatingCount = 156
            },
            new PodcastViewModel
            {
                Id = 8,

                Title = "Nhân quả và sức khỏe",
                Description = "Theo cách nhìn nhà Phật là mình bị ốm vì do mình đã từng làm hại đến sức khỏe người khác. Đấy! Mình bị ốm nặng vì mình đã từng làm hại nặng nề đến sức khỏe người khác. Mình bị ốm chết bởi vì mình đã không chỉ làm hại sức khỏe mà mình còn giết hại người khác. Nên là tất cả những cái bệnh tật đều đến từ chỗ đấy.",
                ImageUrl = "/image/podcast/TraDamTrongSuot.png",
                AudioUrl = "/Audio/SucKhoe.mp3",
                Author = "Trà Đàm Trong Suốt",
                Duration = "139:50",
                Category = "Phát triển bản thân",
                Rating = 4.5,
                RatingCount = 132
            },
            new PodcastViewModel
            {
                Id = 9,
                Title = "Life Update: Cuộc sống của mình sau Podcast",
                Description = "Cuộc đời không có sẵn hướng dẫn, nhưng chúng ta có thể học hỏi từ kinh nghiệm của người khác. Podcast này tập hợp những lời khuyên và hướng dẫn quý giá cho cuộc sống.",
                ImageUrl = "/image/podcast/ThePresent2.png",
                AudioUrl = "/Audio/LifeUpdate.mp3",
                Author = "The Present Writing",
                Duration = "33:40",
                Category = "Phát triển bản thân",
                Rating = 4.6,
                RatingCount = 175
            },
            new PodcastViewModel
            {
                Id = 10,
                Title = "Buổi 13: Hòa Trong Sẽ Thuận Ngoài",
                Description = "Chúng con thương mời Quý đại chúng cùng lắng nghe bài giảng với chủ đề “Hòa trong sẽ thuận ngoài\" do Thầy Minh Niệm chia sẻ, thuộc Chuỗi Livestream “Tay Phật trong tay con” - Buổi 13.",
                ImageUrl = "/image/podcast/MinhNiem13.png",
                AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-10.mp3",
                Author = "Minh Niệm",
                Duration = "60:24",
                Category = "Thiền & Chánh niệm",
                Rating = 4.9,
                RatingCount = 210
            },
            new PodcastViewModel
            {
                Id = 11,
                Title = "Buổi 11: Chấp nhận ánh sáng và bóng tối bên trong",
                Description = "Chúng con thương mời Quý đại chúng cùng lắng nghe bài giảng với chủ đề “Chấp nhận cả ánh sáng và bóng tối bên trong\" do Thầy Minh Niệm chia sẻ, thuộc Chuỗi Livestream “Tay Phật trong tay con” - Buổi 11.",
                ImageUrl = "/image/podcast/MinhNiem11.png",
                AudioUrl = "/Audio/videoplayback.m4a",
                Author = "Minh Niệm",
                Duration = "45:42",
                Category = "Phát triển bản thân",
                Rating = 4.7,
                RatingCount = 158
            },
            new PodcastViewModel
            {
                Id = 12,
                Title = "Mình bị mất kết nối",
                Description = "Thực ra mình đã ẩn số này khi thấy ổn hơn, nhưng mình quyết định republish vì cuộc sống không ngừng lại và cuộc sống của mình cũng vậy, không phải khi mình sáng suốt hơn thì mình sẽ không phạm sai lầm, quan trọng là chúng mình vẫn tiếp tục và chưa bỏ cuộc!\r\n*gắn link không được nên các bạn search tiêu đề này trên youtube nhé “BTS speech at the United Nations”",
                ImageUrl = "/image/podcast/MatKetNoi.png",
                AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-12.mp3",
                Author = "Mây Kể Chuyện",
                Duration = "08:07",
                Category = "Thiền & Chánh niệm",
                Rating = 4.8,
                RatingCount = 187
            }
        };

        public IActionResult Index(string category = null, int page = 1, string search = null)
        {
            int pageSize = 8;
            var query = _podcasts.AsQueryable();

            // Filter by category if provided
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // Filter by search term if provided
            if (!string.IsNullOrEmpty(search))
            {

                query = query.Where(p => 
                    p.Title.Contains(search, System.StringComparison.OrdinalIgnoreCase) || 

                    p.Description.Contains(search, System.StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, System.StringComparison.OrdinalIgnoreCase));
            }

            // Get distinct categories for filter tags
            ViewBag.Categories = _podcasts.Select(p => p.Category).Distinct().ToList();

            
            // Get total count for pagination
            int totalItems = query.Count();
            int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            

            // Apply pagination
            var podcasts = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pagination data for view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentCategory = category;
            ViewBag.CurrentSearch = search;

            return View(podcasts);
        }


        public static List<PodcastViewModel> GetPodcasts()
        {
            return _podcasts;
        }

        public IActionResult Detail(int id = 1)
        {
            var podcast = _podcasts.FirstOrDefault(p => p.Id == id) ?? _podcasts.First();
            

            // Get related podcasts (same category but different ID)
            var relatedPodcasts = _podcasts
                .Where(p => p.Id != podcast.Id && p.Category == podcast.Category)
                .Take(4)
                .ToList();

            // If not enough related podcasts in the same category, add some from other categories
            if (relatedPodcasts.Count < 4)
            {
                var additionalPodcasts = _podcasts
                    .Where(p => p.Id != podcast.Id && p.Category != podcast.Category)
                    .Take(4 - relatedPodcasts.Count)
                    .ToList();

                relatedPodcasts.AddRange(additionalPodcasts);
            }

            ViewBag.RelatedPodcasts = relatedPodcasts;

            return View(podcast);
        }
    }
}
